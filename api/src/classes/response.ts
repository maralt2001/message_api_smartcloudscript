
export class DaysLeft {


    from: String
    to: String
    left: Number

    constructor() {

        let thisYear = new Date().getFullYear();
        this.from = new Date(thisYear,0,1).toDateString();
        this.to = new Date().toDateString();
        this.left = this.daysBetween(new Date(thisYear,0,1), new Date());

    }

    private daysBetween(startDate:any, endDate:any):Number {

        const oneDay = 1000 * 60 * 60 * 24; //in ms
        const differenceMs = Math.abs(startDate - endDate); //in ms
        return Math.round(differenceMs / oneDay)
    
    
    }
}