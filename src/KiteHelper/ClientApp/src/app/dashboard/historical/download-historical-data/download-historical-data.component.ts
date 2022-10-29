import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-download-historical-data',
  templateUrl: './download-historical-data.component.html',
  styleUrls: ['./download-historical-data.component.css']
})
export class DownloadHistoricalDataComponent implements OnInit {


  @Input() public tradingSymbol: string;

  public intervals = ["minute", "5minute", "10minute", "15minute", "day"]


  

  public startDate: Date;
  public endDate: Date;
  public interval: string;
  public downloadSpinner: false;
  constructor() {
  }

  ngOnInit(): void {
    // this.startDate = new Date();
    // this.endDate = new Date();
    this.interval = this.intervals[0];
  }

}
