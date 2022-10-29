import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';
import { ConfigService } from '../config.service';

@Injectable({
  providedIn: 'root',
})
export class KiteApiService {

  private baseUrl: string;
  constructor(
    private configService: ConfigService,
    private cookieService: CookieService,
    private httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public login(request: KiteLoginRequestModel): Observable<HttpResponse<string>> {
    return this.httpClient.post<string>(
      this.baseUrl + this.configService.getConfig().KiteHelperApi.Kite.Login.Endpoint,
      JSON.stringify(request),
      {
        headers: new HttpHeaders({
          observe: 'response',
          'Content-Type': 'application/json',
        }),
        observe: 'response',
      },
    );
  }

  public profile(): Observable<HttpResponse<ProfileResponseModel>> {
    return this.httpClient.get<ProfileResponseModel>(
      this.baseUrl + this.configService.getConfig().KiteHelperApi.Kite.Profile.Endpoint,
      {
        headers: new HttpHeaders({
          Authorization: this.cookieService.get('Authorization'),
          observe: 'response',
          'Content-Type': 'application/json',
        }),
        observe: 'response',
      },
    ).pipe();
  }

  public tradingSymbols(request: TradingSymbolsRequestModel): Observable<HttpResponse<Array<string>>> {
    let httpParam = new HttpParams();
    httpParam = httpParam.append('exchange', request.exchange);
    httpParam = httpParam.append('tradingSymbol', request.tradingSymbol);
    return this.httpClient.get<Array<string>>(
      this.baseUrl + this.configService.getConfig().KiteHelperApi.Kite.TradingSymbols.Endpoint,
      {
        headers: new HttpHeaders({
          Authorization: this.cookieService.get('Authorization'),
          observe: 'response',
          'Content-Type': 'application/json',
        }),
        observe: 'response',
        params: httpParam,
      },
    ).pipe();
  }

  public historicalData(request: HistoricalDataRequestModel): Observable<HttpResponse<Array<string>>> {
    let httpParam = new HttpParams();
    httpParam = httpParam.append('exchange', request.exchange);
    httpParam = httpParam.append('tradingSymbol', request.tradingSymbol);
    httpParam = httpParam.append('startDateTime', request.startDateTime.toUTCString());
    httpParam = httpParam.append('endDateTime', request.endDateTime.toUTCString());
    httpParam = httpParam.append('interval', request.interval);
    return this.httpClient.get<Array<string>>(
      this.baseUrl + this.configService.getConfig().KiteHelperApi.Kite.TradingSymbols.Endpoint,
      {
        headers: new HttpHeaders({
          Authorization: this.cookieService.get('Authorization'),
          observe: 'response',
          'Content-Type': 'application/json',
        }),
        observe: 'response',
        params: httpParam,
      },
    ).pipe();
  }
}

export class KiteLoginRequestModel {
  userName: string;
  password: string;
  appCode: number;
}

export class ProfileResponseModel {
  products: string[];
  userName: string;
  userShortName: string;
  avatarURL: string;
  broker: string;
  userType: string;
  exchanges: string[];
  orderTypes: string[];
  email: string;
}

export class TradingSymbolsRequestModel {
  exchange: string;
  tradingSymbol: string;
}

export class HistoricalDataRequestModel {
  exchange: string;
  tradingSymbol: string;
  startDateTime: Date;
  endDateTime: Date;
  interval: 'minute' | '5minute' | '10minute' | '15minute' | 'day' | string;
}
