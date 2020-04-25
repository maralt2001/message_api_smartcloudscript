"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
//Provide environment variables
require('dotenv').config();
//app setup
const app = express_1.default();
const port = process.env.PORT;
app.get('/api/date/daysleft', (req, res, next) => {
    const thisYear = new Date().getFullYear();
    const today = new Date();
    if (thisYear != undefined && today != undefined) {
        let amountOfDays = daysBetween(new Date(thisYear, 0, 1), today);
        res.status(200).json({ DaysLeft: amountOfDays });
    }
    else {
        // Bad Request
        res.status(400).json({ BadRequest: 'something went wrong' });
    }
});
function daysBetween(startDate, endDate) {
    const oneDay = 1000 * 60 * 60 * 24; //in ms
    const differenceMs = Math.abs(startDate - endDate); //in ms
    return Math.round(differenceMs / oneDay);
}
app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
