
import express, {Request, Response, NextFunction} from 'express'

import {DaysThisYear} from './classes/response'
import {RequestLimiter} from './classes/requestLimiter'
import {ILoginBackendAdmin, IRegisterBackendAdmin} from './classes/requestAdmin'




//Provide environment variables
require('dotenv').config();

//app setup
const app = express();
const bodyParser = require('body-parser');
const router = express.Router();
const port = process.env.PORT;
const fetch = require('node-fetch');

// parse application/x-www-form-urlencoded
app.use(bodyParser.urlencoded({ extended: false }));
// parse application/json
app.use(bodyParser.json());




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

app.get('/api/admin/backendadmins', async (req:Request, res:Response, next:NextFunction) => {

    const myHeaders = new Headers();

    myHeaders.append('Content-Type', 'application/json');
    myHeaders.append('Authorization', `Bearer ${req.headers.authorization}`);


    const result = fetch('http://backend_api/api/admin/backendadmins', {method: 'GET', headers: myHeaders});
    console.log(req.headers.authorization);
    const body = await result;
    console.log(body);
    res.status(200).json(body);
});

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

app.post('/api/admin/backend/login', async(req:Request, res:Response, next:NextFunction) => {

    const body:ILoginBackendAdmin = req.body
    var result = await fetch('http://backend_api/api/admin/backend/login', {
        method: "post",
        body: JSON.stringify(body),
        headers: { "Content-Type": "application/json" }
    });
    const response = await result.json();
    res.status(200).json(response);
});


app.post('/api/admin/backend/register', async(req:Request, res:Response, next:NextFunction) => {

    const body:IRegisterBackendAdmin = req.body;
    var result = await fetch('http://backend_api/api/admin/backend/register', {
        method: "post",
        body: JSON.stringify(body),
        headers: { "Content-Type": "application/json" }
    });
    const response = await result.json();
    res.status(200).json(response);

});

app.get('/api/admin/backend/vault/pathinfo', async(req:Request, res:Response, next:NextFunction) => {

    const result = await fetch('http://backend_api/api/admin/vault/pathinfo');
    const body = await result.json();
    res.status(200).json(body);
})

app.get('/api/admin/backend/metrics', async(req:Request, res:Response, next:NextFunction) => {
    
    const result = await fetch('http://backend_api/api/admin/backend/metrics');
    const body = await result.text();
    console.log(body);
    res.status(200).send(body);
});






app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
