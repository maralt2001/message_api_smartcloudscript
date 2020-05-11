
import express, {Request, Response, NextFunction} from 'express'

import {DaysThisYear} from './classes/response'
import {RequestLimiter} from './classes/requestLimiter'



//Provide environment variables
require('dotenv').config();

//app setup
const app = express();
const port = process.env.PORT;
const fetch = require('node-fetch');

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

app.get('/api/airports', async (req:Request, res:Response, next:NextFunction) => {

    try {
        const result = await fetch('http://backend_api/api/airports');
        const body = await result.json();
        res.status(200).json(body);
        
    } catch (error) {
        res.status(400).json({BadRequest: 'something went wrong'});
    }
    
})

app.get('/api/airport/:id', async (req:Request, res:Response, next:NextFunction) => {

    try {

        const id = req.params.id;
        const result = await fetch(`http://backend_api/api/airport/${id}`);
        const body = await result.json();
        res.status(200).json(body);
        
    } catch (error) {
        
        res.status(400).json({BadRequest: 'something went wrong'})
    }
    

})


app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
