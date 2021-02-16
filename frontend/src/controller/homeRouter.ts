var express = require('express')
var router = express.Router()
import {Request, Response, NextFunction} from 'express'

router.get('/', function(req:Request,res:Response) {

    res.send("<h3>Date Calculator API @smartcloudscript.de/api/date</h3>")
});

module.exports = router
