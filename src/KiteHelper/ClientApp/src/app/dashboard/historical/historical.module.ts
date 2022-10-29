import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HistoricalRoutingModule } from './historical-routing.module';
import { HistoricalDataComponent } from './historical-data/historical-data.component';
import { HistoricalComponent } from './historical.component';
import { NbAutocompleteModule, NbButtonModule, NbCardModule, NbDatepickerModule, NbDialogModule, NbIconModule, NbInputModule, NbRadioModule, NbSpinnerModule, NbTimepickerModule } from '@nebular/theme';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../shared/shared.module';
import { DownloadHistoricalDataComponent } from './download-historical-data/download-historical-data.component';


@NgModule({
  declarations: [
    HistoricalComponent,
    HistoricalDataComponent,
    DownloadHistoricalDataComponent,
  ],
  imports: [
    CommonModule,
    HistoricalRoutingModule,
    FormsModule,
    NbIconModule,
    NbCardModule,
    NbAutocompleteModule,
    NbSpinnerModule,
    NbInputModule,
    NbButtonModule,
    SharedModule,
    NbRadioModule,
    NbDialogModule,
    NbDatepickerModule,
    NbTimepickerModule,
  ],
})
export class HistoricalModule { }
