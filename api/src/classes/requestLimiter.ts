
import {Request} from 'express'



export abstract class RequestLimiter {

    
    public static requestlist:Array<IHttpRequestItem> = [];
    public static blocklist:Array<IBlacklistItem> = [];

    public static isRequestLimitReached(req:Request):Boolean {
        
        let remoteAddress = req.connection.remoteAddress;
        let currentDate:Date = new Date();

        // check condition is ip address in blocklist
        if(this.blocklist.find(r => r.remoteip == remoteAddress)) {
            console.log(this.blocklist, this.requestlist)
            return true;
        }

        // if this condition is true make the first entry in request list
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
                    console.log(this.requestlist)
                    return false;
                }
                // we have spam more then 10 calls in one minute - add request ip to blacklist
                else {
                    this.blocklist.push(this.requestlist[index])
                    this.requestlist = [];
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