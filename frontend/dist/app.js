"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
var ip2loc = require("ip2location-nodejs");
const response_1 = require("./classes/response");
const requestLimiter_1 = require("./classes/requestLimiter");
//Provide environment variables
require('dotenv').config();
//app setup
const app = express_1.default();
const port = process.env.PORT;
app.get('/', (req, res, next) => {
    res.send("<h3>Date Calculator API @smartcloudscript.de/api/date</h3>");
});
app.get('/api/date/days-this-year', (req, res, next) => {
    const responseDays = new response_1.DaysThisYear();
    const limit = requestLimiter_1.RequestLimiter.isRequestLimitReached(req);
    if (limit != true && responseDays != undefined) {
        res.status(200).json(responseDays);
    }
    else {
        // Bad Request
        res.status(400).json({ BadRequest: 'something went wrong' });
    }
});
app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
