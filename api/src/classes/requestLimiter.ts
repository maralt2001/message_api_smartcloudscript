
import {Request} from 'express'
let schedule = require('node-schedule');

// job to delete items from blacklist gte 1 minute
let j = schedule.scheduleJob('*/1 * * * *', function(){
    let lengthBlock = RequestLimiter.blocklist.length
    let blocklistTimer:number = parseInt(process.env.BLOCKLISTTIMER!);
    if(lengthBlock > 0) {
        RequestLimiter.blocklist.forEach((item,index) => {
            if(secondsBetween(item.lastCall, new Date()) >= blocklistTimer) {
                RequestLimiter.blocklist.splice(index,1);
                
            }
        });
    }
    
  });

export abstract class RequestLimiter {

    
    public static requestlist:Array<IHttpRequestItem> = [];
    public static blocklist:Array<IBlacklistItem> = [];
    public static dailylist:Array<IDailyListItem> = [];

    public static isRequestLimitReached(req:Request):Boolean {
        
        let remoteAddress:string = '';
        if(req.headers['x-forwarded-for'] != undefined) {
            remoteAddress = req.headers['x-forwarded-for'] as string
        } 
        else {
            remoteAddress = req.connection.remoteAddress!
        }
        console.log(remoteAddress)
        let currentDate:Date = new Date();

        // check condition is ip address in blocklist
        if(this.blocklist.find(r => r.remoteip == remoteAddress)) {
            console.log(this.blocklist)
            return true;
        }

        // if this condition is true do the first entry in request list
        if(remoteAddress != null && this.requestlist.length == 0) {

            let item:IHttpRequestItem = {remoteip: remoteAddress , lastCall: currentDate, count: 1};
            this.requestlist.push(item);
            this.setRequestToDaily(req);
            console.log('first entry')
            return false;
        }
        // check condition is the ip in request list
        if(this.requestlist.findIndex(r => r.remoteip == remoteAddress) != -1) {
            let index = this.requestlist.findIndex(r => r.remoteip == remoteAddress);
            let lastCall = this.requestlist[index].lastCall;

                // check condition is request in between 60s and counter lt 10
                if(secondsBetween(lastCall,currentDate)< 60 && this.requestlist[index].count < 10) {
                    this.requestlist[index].count++;
                    this.setRequestToDaily(req);
                    console.log(this.dailylist)
                    return false;
                }
                // check condition is request gt 60s and counter lt 10
                else if(secondsBetween(lastCall,currentDate) > 60 && this.requestlist[index].count < 10) {
                    this.requestlist[index].count = 1;
                    this.requestlist[index].lastCall = currentDate;
                    this.setRequestToDaily(req);
                    return false;
                }
                // we have spam more then 10 calls in one minute - add request ip to blacklist
                else {
                    let blacklistItem = this.requestlist[index];
                    this.blocklist.push({remoteip: blacklistItem.remoteip, lastCall: blacklistItem.lastCall})
                    this.requestlist.splice(index,1);
                    this.setRequestToDaily(req);
                    return true;
                }
            
        }
        // check condition is not in requstlist push item to list
        else if (this.requestlist.findIndex(r => r.remoteip == remoteAddress) == -1 && remoteAddress != undefined) {
            let item:IHttpRequestItem = {remoteip: remoteAddress , lastCall: currentDate, count: 1};
            console.log('first entry')
            this.requestlist.push(item);
            this.setRequestToDaily(req);
            return false;
        }
        else {
            
            return true;
        }

        
        
    }

    private static setRequestToDaily(req:Request) {

        let reqip = req.connection.remoteAddress;
        let currentDate = new Date();
        
        if(req != undefined) {

            // set new entry no item exists
            if(this.dailylist.findIndex(r => r.remoteip == reqip) == -1 && reqip != undefined) {
                let newItem:IDailyListItem = {remoteip: reqip, count: 1, lastcall: new Date(), timetodelete: addOneDay(currentDate)};
                this.dailylist.push(newItem);
            }
            // item exist update properties
            else {
                let index = this.dailylist.findIndex(r => r.remoteip == reqip)
                // condition if the same day count up
                if(isSameDay(this.dailylist[index].lastcall)) {
                    this.dailylist[index].count++;
                    this.dailylist[index].lastcall = new Date();
                }
                // condition if not the same day resest counter
                else {
                    this.dailylist[index].count == 1;
                    this.dailylist[index].lastcall = new Date()
                }

                
            }
            
        }
        else {
            return;
        }

    }

}

interface IHttpRequestItem {

    remoteip:string 
    lastCall: Date 
    count: number
}

interface IBlacklistItem {

    remoteip: string
    lastCall: Date

}

interface IDailyListItem {
    remoteip: string
    count: number
    lastcall: Date
    timetodelete: Date
}

function secondsBetween(startDate:any, endDate:any):Number {

    const oneMinute = 1000; //in ms
    const differenceMs = Math.abs(startDate - endDate); //in ms
    return Math.round(differenceMs / oneMinute)

}

function addOneDay(startDate:any):Date {

    var result = new Date(startDate);
    result.setDate(result.getDate() + 1);
    return result;
}

function isSameDay(startDate:Date):Boolean {

    let today = new Date();
    return today.getFullYear() == startDate.getFullYear() && today.getMonth() == startDate.getMonth() && today.getDate() == startDate.getDate();

}