import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConfigService } from '../config.service';
import { CookieHelperService } from '../cookie-helper.service';
import { HelperService } from '../helper.service';

@Injectable({
  providedIn: 'root',
})
export class KiteApiService {

  private baseUrl: string;
  constructor(
    private configService: ConfigService,
    private cookieHelperService: CookieHelperService,
    private helperService: HelperService,
    private httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  public login(request: KiteLoginRequestModel): Observable<HttpResponse<KiteLoginResponseModel>> {
    return this.httpClient.post<KiteLoginResponseModel>(
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
          Authorization: this.cookieHelperService.getAuthorizationToken(),
          observe: 'response',
          'Content-Type': 'application/json',
        }),
        observe: 'response',
      },
    ).pipe();
  }

  public tradingSymbols(request: TradingSymbolsRequestModel): Observable<HttpResponse<Array<TradingSymbolResponseModel>>> {
    let httpParam = new HttpParams();
    httpParam = httpParam.append('TradingSymbol', request.TradingSymbol);
    return this.httpClient.get<Array<TradingSymbolResponseModel>>(
      this.baseUrl + this.configService.getConfig().KiteHelperApi.Kite.TradingSymbols.Endpoint,
      {
        headers: new HttpHeaders({
          Authorization: this.cookieHelperService.getAuthorizationToken(),
          observe: 'response',
          'Content-Type': 'application/json',
        }),
        observe: 'response',
        params: httpParam,
      },
    ).pipe();
  }

  public historicalData(request: HistoricalDataRequestModel): Observable<HttpResponse<Array<HistoricalDataResponseModel>>> {

    const reqBody = {
      Exchange: request.Exchange,
      TradingSymbol: request.TradingSymbol,
      StartDateTime: request.StartDateTime.toISOString(),
      EndDateTime: request.EndDateTime.toISOString(),
      Interval: request.Interval,
    };

    return this.httpClient.post<Array<HistoricalDataResponseModel>>(
      this.baseUrl + this.configService.getConfig().KiteHelperApi.Kite.HistoricalData.Endpoint,
      JSON.stringify(reqBody),
      {
        headers: new HttpHeaders({
          Authorization: this.cookieHelperService.getAuthorizationToken(),
          observe: 'response',
          'Content-Type': 'application/json',
        }),
        observe: 'response',
      },
    ).pipe();
  }
}

export class KiteLoginRequestModel {
  UserName: string;
  Password: string;
  AppCode: number;
}

export class KiteLoginResponseModel {
  SessionId: string;
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
  TradingSymbol: string;
}

export class TradingSymbolResponseModel {
  Id: number;
  InstrumentToken: string;
  ExchangeToken: string;
  TradingSymbol: string;
  Name: string;
  LastPrice: number;
  Expiry?: Date;
  Strike: string;
  TickSize: number;
  LotSize: number;
  InstrumentType: string;
  Segment: string;
  Exchange: string;
}
export class HistoricalDataRequestModel {
  Exchange: string;
  TradingSymbol: string;
  StartDateTime: Date;
  EndDateTime: Date;
  Interval: 'minute' | '5minute' | '10minute' | '15minute' | 'day' | string;
}

export class HistoricalDataResponseModel {
  TimeStamp: string;
  Open: number;
  High: number;
  Low: number;
  Close: number;
  Volume: number;
  OI: number;
}
