using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using backend_api.Controllers;
using backend_api.Database;
using backend_api.Tracker;
using backend_api.Vault.Models;
using Quartz;
using Quartz.Impl;
using VaultSharp;
using VaultSharp.Core;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using static backend_api.Database.DBContext;

namespace backend_api.Vault
{
    public class VaultAccess : IVaultAccess
    {

        public static string BaseUrl { get; set; }
        public static string PathSealState { get; set; }
        public static int HealthCheckTimer { get; set; }
        public static string AdminToken { get; set; }
        public static VaultClient AdminClient { get; set; }
        public static VaultClient DBClient { get; set; }
        public static string DBPolicy { get; set; }
        public static VaultSolution VaultAdminClientSolution { get; set; }
        public static VaultSolution VaultDBClientSolution { get; set; }

        public async void CreateMongoWithVaultCredentials()
        {

            
            var check = typeof(IDBContext).IsAssignableFrom(typeof(MongoWithCredentialVault));
            if(check)
            {
                
                var result = await GetSecret("db/login", "secret");
                var dblogin = result.Data;
                
                AdminController.mongoWithCredentialVault = new MongoWithCredentialVault("WebDB", "backend_db", dblogin.user, dblogin.password);
                Console.WriteLine("Mongo: created Mongo Client with vault credentials");
                
            }

        }

        public async Task<Secret<DBLoginFromVault>> GetSecret(string path, string mountpoint)
        {
            try
            {
                // path:db/login/ mountpoint:secret
                var secret = await DBClient.V1.Secrets.KeyValue.V1.ReadSecretAsync<DBLoginFromVault>(path, mountpoint);
                return secret;
            }
            catch (VaultApiException e)
            {
                Console.WriteLine(e.Message);
                return new Secret<DBLoginFromVault>();
            }
        }

        public async Task<ListInfo> GetPathInfo(string mountpoint, string path)
        {
            try
            {
                Secret<ListInfo> secret = await DBClient.V1.Secrets.KeyValue.V1.ReadSecretPathsAsync(path, mountpoint);
                return secret.Data;
            }
            catch (Exception)
            {

                return new ListInfo();
            }
        }
        protected async Task<bool> CheckAdminAuthentication()
        {
            try
            {
                var lookup = await AdminClient.V1.Auth.Token.LookupSelfAsync();
                return true;

            }
            catch (VaultApiException e)
            {
                return !e.ApiErrors.Contains<string>("permission denied");

            }

        }

    }

    public class AdminClientVault : VaultAccess
    {
        
        public AdminClientVault()
        {
            
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(AdminToken);
            var vaultClientSettings = new VaultClientSettings(BaseUrl, authMethod);
            AdminClient = new VaultClient(vaultClientSettings);
            
            VaultAdminClientSolution = new VaultSolution { VaultItem = VaultItem.VaultAdminClient};
            
            VaultAdminClientSolution.PropertyChanged += VaultAdmin_PropertyChanged;
            VaultAdminClientSolution.Authenticated = CheckAdminAuthentication().Result;

        }


        

        private void VaultAdmin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            if(e.PropertyName == "ConnectionUp")
            {
                Console.WriteLine($"Vault: Connection to {BaseUrl}{PathSealState} is up {VaultAdminClientSolution.ConnectionUp} ");
            }
            if(e.PropertyName == "Sealed")
            {
                Console.WriteLine($"Vault: is Sealed {VaultAdminClientSolution.Sealed}");
            }
            if(e.PropertyName == "Authenticated")
            {
                Console.WriteLine($"Vault: Admin is Authenticated {VaultAdminClientSolution.Authenticated}");
                
            }
            
        }
    }

    public class DBClientVault : VaultAccess
    {
        
        public DBClientVault(string policy)
        {
           
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(GetAppToken(policy).GetAwaiter().GetResult());
            var vaultClientSettings = new VaultClientSettings(BaseUrl, authMethod);
            DBClient = new VaultClient(vaultClientSettings);
            VaultDBClientSolution = new VaultSolution { VaultItem = VaultItem.VaultDBClient };
            VaultDBClientSolution.PropertyChanged += VaultDB_PropertyChanged;
            
        }

        private void VaultDB_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            
            
        }

        private async Task<string> GetAppToken(string policy)
        {
            CreateTokenRequest request = new CreateTokenRequest

            {
                Renewable = true,
                Policies = new List<string> { policy },
                TimeToLive = "1h"
            };
            Secret<object> tokenData = await AdminClient.V1.Auth.Token.CreateTokenAsync(request);
            return tokenData.AuthInfo.ClientToken;
           
        }

    }
    public class VaultJobScheduler: VaultAccess 
    {
       
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<VaultStateJob>().Build();

            ITrigger trigger = TriggerBuilder.Create().StartNow().WithSimpleSchedule(s => s.WithIntervalInSeconds(HealthCheckTimer).RepeatForever()).Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }

    

    public class VaultStateJob : VaultAccess, IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            
            //check Instance of VaultAdminClient
            if (VaultAdminClientSolution == null)
            {
                new AdminClientVault();
                
            }

            if(VaultDBClientSolution == null && VaultAdminClientSolution.Authenticated && VaultAdminClientSolution.Sealed != true)
            {
                new DBClientVault(DBPolicy);
                CreateMongoWithVaultCredentials();


            }

            try
            {
                await VaultAdminClientSolution.CheckConnection(BaseUrl, PathSealState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
