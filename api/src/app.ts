
import express, {Request, Response, NextFunction, response} from 'express'
var ip2loc = require("ip2location-nodejs");

import {DaysThisYear} from './classes/response'
import {RequestLimiter} from './classes/requestLimiter'


//Provide environment variables
require('dotenv').config();

//app setup
const app = express();
const port = process.env.PORT;

app.get('/', (req:Request,res:Response, next:NextFunction) => {

    res.send("<h3>Date Calculator API @smartcloudscript.de/api/date</h3>")
});

app.get('/api/date/days-this-year', (req:Request,res:Response, next:NextFunction) => {
    
    const responseDays = new DaysThisYear();
    const limit = RequestLimiter.isRequestLimitReached(req);

    if(limit != true && responseDays != undefined) {
        res.status(200).json(responseDays);
    }

    else {
        // Bad Request
        res.status(400).json({BadRequest: 'something went wrong'})
    }

});

app.get('/api/location/' ,(req:Request, res:Response, next:NextFunction ) => {

    ip2loc.IP2Location_init("./database/ip2location/ip2location.BIN");
 
    let testip = [`${req.connection.remoteAddress}`];
    testip.forEach((element,index) => {
        let result = ip2loc.IP2Location_get_all(testip[index]);
        res.status(200).json({country: result.country_short, region: result.region, timezone: result.timezone, domain: result.domain, zipcode: result.zipcode})
    });
    

})

app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
