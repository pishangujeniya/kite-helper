import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-trading-symbol',
  templateUrl: './trading-symbol.component.html',
  styleUrls: ['./trading-symbol.component.scss'],
})
export class TradingSymbolComponent implements OnInit {

  @Input() public tradingSymbol: string;
  @Input() public name: string;
  @Input() public expiry: string;
  @Input() public instrumentType: string;
  @Input() public exchange: string;
  @Input() public strike: string;

  public expiryDate: Date;

  constructor() { }

  ngOnInit(): void {
    if (this.expiry === null || this.expiry === undefined || this.expiry.length < 1) {
      this.expiryDate = new Date();
    } else {
      this.expiryDate = new Date(this.expiry);
    }
  }

}
