import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotFoundComponent } from './not-found/not-found.component';
import { NbAlertModule, NbButtonModule, NbCardModule, NbProgressBarModule, NbSpinnerModule } from '@nebular/theme';
import { TradingSymbolComponent } from './trading-symbol/trading-symbol.component';


@NgModule({
  declarations: [NotFoundComponent, TradingSymbolComponent],
  imports: [
    CommonModule,
    NbCardModule,
    NbButtonModule,
    NbAlertModule,
    NbSpinnerModule,
    NbProgressBarModule,
  ],
  exports: [
    NotFoundComponent,
    TradingSymbolComponent,
  ],
})
export class SharedModule { }
