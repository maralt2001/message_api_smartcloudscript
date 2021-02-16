var express = require('express')
var router = express.Router()
import {Request, Response, NextFunction} from 'express'
const fetch = require('node-fetch');

router.get('/backendadmins', async (req:Request, res:Response, next:NextFunction) => {

    try {
        const result = await fetch('http://backend_api/api/admin/backendadmins', {
            method: 'GET',
            headers: {
                'content-type': 'application/json',
                'Authorization': `${req.headers.authorization}`
            }
            });
            const body = await result.json();
            res.status(200).json(body) 
    } catch (error) {
        res.status(400).json({BadRequest: 'something went wrong'});
    }

});

router.get('/backendadmins/count', async (req:Request, res:Response, next:NextFunction) => {

    try {
        const query = req.query;
        var key = Object.keys(query).toString();
        var value = Object.values(query).toString();
        const result = await fetch(`http://backend_api/api/admin/backendadmins/count?${key}=${value}`, {
            method: 'GET',
            headers: {
                'content-type': 'application/json',
                'Authorization': `${req.headers.authorization}`
            }
        });
        const body = await result.json();
        res.status(200).json(body);
    } catch (error) {
        res.status(400).json({BadRequest: 'something went wrong'});
    }
})

router.get('/dbstatus', async (req:Request, res:Response, next:NextFunction) => {

    const result = await fetch('http://backend_api/api/admin/dbstatus');
    const body = await result.json();
    res.status(200).json(body);
});

router.get('/db/bulkinsert', async(req:Request, res:Response, next:NextFunction) => {

    const query = req.query
    const result = await fetch(`http://backend_api/api/admin/job/bulkinsert?filename=${query.filename}`);
    const body = await result.json();
    res.status(200).json(body);
})

router.get('/db/createindex', async(req:Request, res:Response, next:NextFunction) => {
    const query = req.query
    var key = Object.keys(query).toString();
    var value = Object.values(query).toString();
    const result = await fetch(`http://backend_api/api/admin/job/airports/createindex?${key}=${value}`)
    const body = await result.json();
    res.status(201).json(body);
})

router.get('/db/dropindex', async(req:Request, res:Response, next:NextFunction) => {
    const query = req.query
    var key = Object.keys(query).toString();
    var value = Object.values(query).toString();
    const result = await fetch(`http://backend_api/api/admin/job/airports/dropindex?${key}=${value}`)
    const body = await result.json();
    res.status(201).json(body);
})

module.exports = router

