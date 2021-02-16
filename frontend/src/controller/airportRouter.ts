var express = require('express')
var router = express.Router()
import {Request, Response, NextFunction} from 'express'
const fetch = require('node-fetch');


router.get('/', async (req:Request, res:Response, next:NextFunction) => {

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
});

module.exports = router