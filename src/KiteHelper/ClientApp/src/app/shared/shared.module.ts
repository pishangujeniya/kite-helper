import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotFoundComponent } from './not-found/not-found.component';
import { NbAlertModule, NbButtonModule, NbCardModule, NbIconModule, NbProgressBarModule, NbSpinnerModule } from '@nebular/theme';
import { TradingSymbolComponent } from './trading-symbol/trading-symbol.component';
import { ErrorDialogComponent } from './error-dialog/error-dialog.component';


@NgModule({
  declarations: [NotFoundComponent, TradingSymbolComponent, ErrorDialogComponent],
  imports: [
    CommonModule,
    NbCardModule,
    NbButtonModule,
    NbAlertModule,
    NbSpinnerModule,
    NbProgressBarModule,
    NbIconModule
  ],
  exports: [
    NotFoundComponent,
    TradingSymbolComponent,
  ],
})
export class SharedModule { }
