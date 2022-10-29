import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-trading-symbol',
  templateUrl: './trading-symbol.component.html',
  styleUrls: ['./trading-symbol.component.css'],
})
export class TradingSymbolComponent implements OnInit {
  @Input() public tradingSymbol: string;
  constructor() { }

  ngOnInit(): void {
  }

}
