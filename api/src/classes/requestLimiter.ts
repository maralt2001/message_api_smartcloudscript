
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

    public static isRequestLimitReached(req:Request):Boolean {
        
        let remoteAddress = req.connection.remoteAddress;
        let currentDate:Date = new Date();

        // check condition is ip address in blocklist
        if(this.blocklist.find(r => r.remoteip == remoteAddress)) {
            return true;
        }

        // if this condition is true do the first entry in request list
        if(remoteAddress != null && this.requestlist.length == 0) {

            let item:IHttpRequestItem = {remoteip: remoteAddress , lastCall: currentDate, count: 0};
            this.requestlist.push(item);
            return false;
        }
        // check condition is the ip in request list
        else if(this.requestlist.findIndex(r => r.remoteip == remoteAddress) != -1) {
            let index = this.requestlist.findIndex(r => r.remoteip == remoteAddress);
            let lastCall = this.requestlist[index].lastCall;

                // check condition is request in between 60s and counter lt 10
                if(secondsBetween(lastCall,currentDate)< 60 && this.requestlist[index].count < 10) {
                    this.requestlist[index].count++;
                    return false;
                }
                // we have spam more then 10 calls in one minute - add request ip to blacklist
                else {
                    let blacklistItem = this.requestlist[index];
                    this.blocklist.push({remoteip: blacklistItem.remoteip, lastCall: blacklistItem.lastCall})
                    //this.requestlist = [];
                    this.requestlist.splice(index,1);
                    return true;
                }
            
        }
        else {
            return true;
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

function secondsBetween(startDate:any, endDate:any):Number {

    const oneMinute = 1000; //in ms
    const differenceMs = Math.abs(startDate - endDate); //in ms
    return Math.round(differenceMs / oneMinute)

}