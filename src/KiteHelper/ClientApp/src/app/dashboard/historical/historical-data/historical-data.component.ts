import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NbDialogService, NbToastrService } from '@nebular/theme';
import { KiteApiService } from 'src/app/services/api/kite-api.service';
import { environment } from 'src/environments/environment';
import { DownloadHistoricalDataComponent } from '../download-historical-data/download-historical-data.component';

@Component({
  selector: 'app-historical-data',
  templateUrl: './historical-data.component.html',
  styleUrls: ['./historical-data.component.css'],
})
export class HistoricalDataComponent implements OnInit {

  // @ViewChild('tradingSymbolInput') input;

  public tradingSymbol: string;

  public filteredOptions: Array<string>;

  constructor(
    private kiteApiService: KiteApiService,
    private toastrService: NbToastrService,
    private dialogService: NbDialogService
  ) { }

  ngOnInit(): void {
    this.filteredOptions = new Array<string>();
  }

  public onTradingSymbolChange() {
    const searchedSymbol: string = this.tradingSymbol;
    if (searchedSymbol.length > 2) {
      this.loadTradingSymbolsList(searchedSymbol);
    } else {
      this.filteredOptions = [];
    }
  }

  public loadTradingSymbolsList(searchedSymbol: string) {
    this.kiteApiService.tradingSymbols(
      {
        exchange: '',
        tradingSymbol: searchedSymbol,
      }).toPromise()
      .then(
        (getInstrumentsTradingSymbolResponseModel: HttpResponse<Array<string>>) => {
          this.filteredOptions = getInstrumentsTradingSymbolResponseModel.body;
        }, (errorResponse: HttpErrorResponse) => {
          console.error(errorResponse);
          if (errorResponse.status > 0) {
            this.toastrService.danger(JSON.stringify(errorResponse.error), environment.defaultErrorTitle);
          } else {
            this.toastrService.danger(environment.defaultErrorMessage, environment.defaultErrorTitle);
          }
        },
      )
      .catch(
        (exception: any) => {
          console.error(exception);
          this.toastrService.danger(environment.defaultErrorMessage, environment.defaultErrorTitle);
        },
      );
  }

  public selectedTradingSymbol(option: any) {
    this.dialogService.open(DownloadHistoricalDataComponent, { hasBackdrop: true });
  }

  onSelectionChange($event) {
    this.tradingSymbol = this.tradingSymbol;
  }

  public searchButtonClick() {
    console.log(this.tradingSymbol);
  }

}
