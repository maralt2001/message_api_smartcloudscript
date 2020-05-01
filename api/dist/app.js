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
    console.log(limit);
    if (limit != true && responseDays != undefined) {
        res.status(200).json(responseDays);
    }
    else {
        // Bad Request
        res.status(400).json({ BadRequest: 'something went wrong' });
    }
});
app.get('/api/location/', (req, res, next) => {
    ip2loc.IP2Location_init("./database/ip2location/ip2location.BIN");
    let testip = ['95.91.236.144'];
    testip.forEach((element, index) => {
        let result = ip2loc.IP2Location_get_all(testip[index]);
        res.status(200).json({ country: result.country_short, region: result.region, timezone: result.timezone, domain: result.domain, zipcode: result.zipcode });
    });
});
app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
//# sourceMappingURL=app.js.map