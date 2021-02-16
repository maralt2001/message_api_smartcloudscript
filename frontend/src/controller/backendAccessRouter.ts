var express = require('express')
var router = express.Router()
import {Request, Response, NextFunction} from 'express'
import {ILoginBackendAdmin, IRegisterBackendAdmin} from '../classes/requestAdmin'
const fetch = require('node-fetch');


router.post('/login', async(req:Request, res:Response, next:NextFunction) => {

    const body:ILoginBackendAdmin = req.body
    var result = await fetch('http://backend_api/api/admin/backend/login', {
        method: "post",
        body: JSON.stringify(body),
        headers: { "Content-Type": "application/json" }
    });
    const response = await result.json();
    res.status(200).json(response);
});

router.post('/register', async(req:Request, res:Response, next:NextFunction) => {

    const body:IRegisterBackendAdmin = req.body;
    var result = await fetch('http://backend_api/api/admin/backend/register', {
        method: "post",
        body: JSON.stringify(body),
        headers: { "Content-Type": "application/json" }
    });
    const response = await result.json();
    res.status(200).json(response);

});

module.exports = router