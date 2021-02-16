var express = require('express')
var router = express.Router()
import {Request, Response, NextFunction} from 'express'

const fetch = require('node-fetch');


router.get('/', async(req:Request, res:Response, next:NextFunction) => {
    
    const result = await fetch('http://backend_api/api/admin/backend/metrics');
    const body = await result.text();
    res.status(200).send(body);
});

module.exports = router