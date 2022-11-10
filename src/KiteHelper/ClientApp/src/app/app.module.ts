import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { APP_INITIALIZER, Injector, NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ThemeModule } from './@theme/theme.module';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import {
  NbDatepickerModule,
  NbDialogModule,
  NbMenuModule,
  NbSidebarModule,
  NbTimepickerModule,
  NbToastrModule,
  NbWindowModule,
} from '@nebular/theme';
import { NbAuthModule } from '@nebular/auth';
import { ConfigService } from './services/config.service';
import { UnauthorizedInterceptor } from './interceptors/unauthorized.interceptor';

const appInitializerFn = (appConfig: ConfigService) => () => appConfig.requestConfig();

export function authInterceptorFactory(injector: Injector) {
  return new UnauthorizedInterceptor(injector);
}

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AppRoutingModule,
    ThemeModule.forRoot(),
    NbSidebarModule.forRoot(),
    NbMenuModule.forRoot(),
    NbDatepickerModule.forRoot(),
    NbTimepickerModule.forRoot(),
    NbDialogModule.forRoot(),
    NbWindowModule.forRoot(),
    NbToastrModule.forRoot(),
    NbAuthModule.forRoot(),
  ],
  providers: [
    ConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: appInitializerFn,
      multi: true,
      deps: [ConfigService],
    },
    UnauthorizedInterceptor,
    {
      provide: HTTP_INTERCEPTORS,
      useFactory: authInterceptorFactory,
      multi: true,
      deps: [Injector, UnauthorizedInterceptor],
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
