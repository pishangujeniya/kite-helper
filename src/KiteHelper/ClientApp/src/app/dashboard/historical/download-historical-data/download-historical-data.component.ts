import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { NbDialogRef, NbToastrService } from '@nebular/theme';
import { HistoricalDataResponseModel, KiteApiService, TradingSymbolResponseModel } from 'src/app/services/api/kite-api.service';
import { HelperService } from 'src/app/services/helper.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-download-historical-data',
  templateUrl: './download-historical-data.component.html',
  styleUrls: ['./download-historical-data.component.css'],
})
export class DownloadHistoricalDataComponent implements OnInit {


  public tradingSymbolResponseModel: TradingSymbolResponseModel;

  public intervals = ['minute', '5minute', '10minute', '15minute', 'day'];

  public startDate: Date;
  public endDate: Date;
  public interval: string;
  public downloadSpinner: boolean;

  constructor(
    private kiteApiService: KiteApiService,
    private toastrService: NbToastrService,
    private helperService: HelperService,
    private dialogRef: NbDialogRef<DownloadHistoricalDataComponent>,
  ) {
  }

  ngOnInit(): void {

    this.startDate = new Date();
    this.startDate.setDate(this.startDate.getDate() - 5);

    this.endDate = new Date();

    this.interval = this.intervals[0];
  }

  public async downloadHistoricalData() {
    this.downloadSpinner = true;
    await this.kiteApiService.historicalData(
      {
        TradingSymbol: this.tradingSymbolResponseModel.TradingSymbol,
        Exchange: this.tradingSymbolResponseModel.Exchange,
        StartDateTime: this.startDate,
        EndDateTime: this.endDate,
        Interval: this.interval,
      },
    ).toPromise()
      .then(
        (response: HttpResponse<Array<HistoricalDataResponseModel>>) => {
          this.helperService.exportToCSVFile(response.body, this.tradingSymbolResponseModel.TradingSymbol, null, null, false);
          this.dialogRef.close();
          this.toastrService.success('Downloaded successfully', this.tradingSymbolResponseModel.TradingSymbol + ' Data');
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
      )
      .finally(() => {
        this.downloadSpinner = false;
      });
  }

}
