
import express, {Request, Response, NextFunction} from 'express'

import {DaysThisYear} from './classes/response'
import {RequestLimiter} from './classes/requestLimiter'



//Provide environment variables
require('dotenv').config();

//app setup
const app = express();
const port = process.env.PORT;
const fetch = require('node-fetch');


const apiAirports = process.env.APIAIRPORTS as string;

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

app.get('/api/airport', async (req:Request, res:Response, next:NextFunction) => {

    try {
        const query = req.query
        var key = Object.keys(query).toString();
        var value = Object.values(query).toString();
        const result = await fetch(`http://backend_api/api/airport?${key}=${value}`);
        const body = await result.json();
        res.status(200).json(body)
    } catch (error) {
        res.status(400).json({BadRequest: 'something went wrong'})
    }
})

app.get('/api/admin/dbstatus', async (req:Request, res:Response, next:NextFunction) => {

    const result = await fetch('http://backend_api/api/admin/dbstatus');
    const body = await result.json();
    res.status(200).json(body);
});

app.get('/api/admin/db/bulkinsert', async(req:Request, res:Response, next:NextFunction) => {

    const query = req.query
    const result = await fetch(`http://backend_api/api/admin/job/bulkinsert?filename=${query.filename}`);
    const body = await result.json();
    res.status(200).json(body);
})

app.get('/api/admin/db/createindex', async(req:Request, res:Response, next:NextFunction) => {
    const query = req.query
    var key = Object.keys(query).toString();
    var value = Object.values(query).toString();
    const result = await fetch(`http://backend_api/api/admin/job/airports/createindex?${key}=${value}`)
    const body = await result.json();
    res.status(201).json(body);
})

app.get('/api/admin/db/dropindex', async(req:Request, res:Response, next:NextFunction) => {
    const query = req.query
    var key = Object.keys(query).toString();
    var value = Object.values(query).toString();
    const result = await fetch(`http://backend_api/api/admin/job/airports/dropindex?${key}=${value}`)
    const body = await result.json();
    res.status(201).json(body);
})




app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
