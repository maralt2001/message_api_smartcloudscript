
import express, {Request, Response, NextFunction} from 'express'

import {DaysLeft} from './classes/response'

//Provide environment variables
require('dotenv').config();

//app setup
const app = express();
const port = process.env.PORT;

app.get('/', (req:Request,res:Response, next:NextFunction) => {

    res.send("<h3>Date Calculator API @smartcloudscript.de/api/date</h3>")
});

app.get('/api/date/daysleft', (req:Request,res:Response, next:NextFunction) => {

    
    const responseDaysLeft = new DaysLeft();

    if(responseDaysLeft != undefined) {
        
        res.status(200).json(responseDaysLeft);
    }
    else {
        
        res.status(400).json({BadRequest: 'something went wrong'})
    }

});



app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
