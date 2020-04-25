
import express, {Request, Response, NextFunction} from 'express'

//Provide environment variables
require('dotenv').config();

//app setup
const app = express();
const port = process.env.PORT;

app.get('/', (req:Request,res:Response, next:NextFunction) => {

    res.send("<h3>Date Calculator API @smartcloudscript.de/api/date</h3>")
});

app.get('/api/date/daysleft', (req:Request,res:Response, next:NextFunction) => {

    const thisYear = new Date().getFullYear();
    const today = new Date();
    
    if(thisYear != undefined && today != undefined)
    {
        let amountOfDays = daysBetween(new Date(thisYear,0,1), today);
        res.status(200).json({From: new Date(2020,0,1).toDateString(), To: today.toDateString(), DaysLeft: amountOfDays});
    }
    else {
        // Bad Request
        res.status(400).json({BadRequest: 'something went wrong'}) 
    }

});

function daysBetween(startDate:any, endDate:any):Number {

    const oneDay = 1000 * 60 * 60 * 24; //in ms
    const differenceMs = Math.abs(startDate - endDate); //in ms
    return Math.round(differenceMs / oneDay)


}

app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
