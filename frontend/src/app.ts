
import express from 'express'

//Provide environment variables
require('dotenv').config();

//app setup
const app = express();
const bodyParser = require('body-parser');
const port = process.env.PORT;

const dateRouter = require('./controller/dateRouter')
const homeRouter = require('./controller/homeRouter')
const airportRouter = require('./controller/airportRouter')
const adminRouter = require('./controller/adminRouter')
const backendAccessRouter = require('./controller/backendAccessRouter')
const backendMetricsRouter = require('./controller/backendMetricsRouter')

// parse application/x-www-form-urlencoded
app.use(bodyParser.urlencoded({ extended: false }));
// parse application/json
app.use(bodyParser.json());

app.use('/api/date', dateRouter);
app.use('/api/airport', airportRouter)
app.use('/api/admin', adminRouter)
app.use('/api/admin/backend', backendAccessRouter)
app.use('/api/admin/backend/metrics', backendMetricsRouter)
app.use('/', homeRouter);

app.listen(port, () => console.log(`Server ist started on port ${port} ...`));
