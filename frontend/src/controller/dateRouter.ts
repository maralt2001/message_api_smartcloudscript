var express = require('express')
var router = express.Router()
import {Request, Response, NextFunction} from 'express'
import {DaysThisYear} from '../classes/response'
import {RequestLimiter} from '../classes/requestLimiter'

router.get('/days-this-year', function(req:Request,res:Response) {

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

module.exports = router

