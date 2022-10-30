import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ConfigService {

  private config: Config;

  constructor(private httpClient: HttpClient) { }

  public async requestConfig() {
    return this.httpClient.get('/config.json').toPromise()
      .then(
        (configJsonSuccessResponse: Config) => {
          this.config = configJsonSuccessResponse;
        },
        (configJsonErrorResponse: HttpErrorResponse) => {
          console.error(configJsonErrorResponse);
        },
      )
      .catch((error: any) => {
        console.error(error);
      });
  }

  public getConfig(): Config {
    return this.config;
  }
}

export class Config {
  KiteHelperApi: {
    Kite: {
      Login: {
        Endpoint: string;
      };
      Profile: {
        Endpoint: string;
      };
      TradingSymbols: {
        Endpoint: string;
      };
      HistoricalData: {
        Endpoint: string;
      };
    };
  };
}
