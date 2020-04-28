
import express, {Request, Response, NextFunction} from 'express'

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
    console.log(limit)
    
    if(responseDays != undefined) {
    
        res.status(200).json(responseDays);
    }
    else {
        // Bad Request
        res.status(400).json({BadRequest: 'something went wrong'})
    }
    
    

    

});

app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
