"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class DaysThisYear {
    constructor() {
        let thisYear = new Date().getFullYear();
        this.from = new Date(thisYear, 0, 1).toDateString();
        this.to = new Date().toDateString();
        this.left = daysBetween(new Date(thisYear, 0, 1), new Date());
        this.remaining = daysBetween(new Date(), new Date(thisYear, 11, 31));
        this.leapyear = calculateLeapyear(thisYear);
    }
}
exports.DaysThisYear = DaysThisYear;
function daysBetween(startDate, endDate) {
    const oneDay = 1000 * 60 * 60 * 24; //in ms
    const differenceMs = Math.abs(startDate - endDate); //in ms
    return Math.round(differenceMs / oneDay);
}
function calculateLeapyear(year) {
    return (year % 100 === 0) ? (year % 400 === 0) : (year % 4 === 0);
}
