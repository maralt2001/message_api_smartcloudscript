using backend_api.Vault.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace backend_api.Tracker
{
    public class VaultSolution : INotifyPropertyChanged
    {
        
        private bool _Authenticated = false;
        private bool _ConnectionUp = false;
        private bool _Sealed = true;
        public VaultItem VaultItem { get; set; }

        public bool Sealed { get { return _Sealed; } set { SetStateSealed(value); } } 

        public bool ConnectionUp { get { return _ConnectionUp; } set { SetStateConnection(value); } }

        public bool Authenticated { get { return _Authenticated; } set { SetStateAuthenticated(value); } }
        
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private void SetStateSealed(bool value)
        {
            if (_Sealed != value)
            { 
                _Sealed = value;
                ChangedAt = DateTime.Now;
                OnPropertyChanged("Sealed");
            }
        }

        private void SetStateConnection(bool value)
        {
            if (_ConnectionUp != value)
            {
                _ConnectionUp = value;
                ChangedAt = DateTime.Now;
                OnPropertyChanged("ConnectionUp");
            }
            
        }

        private void SetStateAuthenticated(bool value)
        {
            if (_Authenticated != value)
            {
                _Authenticated = value;
                ChangedAt = DateTime.Now;
                OnPropertyChanged("Authenticated");
            }
        }

        public  async Task CheckConnection(string BaseUrl, string PathSealState)
        {
            using var client = new HttpClient();
            try
            {
                var response = await client.GetAsync(BaseUrl + PathSealState);
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<VaultSealed>(content);

                if (result.initialized)
                {
                    ConnectionUp = true;

                    if (result.@sealed)
                    {
                        Sealed = true;

                    }
                    else
                    {
                        Sealed = false;
                    }
                    
                }
                else
                {
                    ConnectionUp = false;
                    
                }

            }
            catch (HttpRequestException)
            {
                ConnectionUp = false;
                Sealed = true;
                

            }
        }


    }

    public enum VaultItem
    {
        VaultConnection,
        VaultAdminClient,
        VaultDBClient
    }
    
}
