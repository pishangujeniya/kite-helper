import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotFoundComponent } from './not-found/not-found.component';
import { NbAlertModule, NbButtonModule, NbCardModule, NbProgressBarModule, NbSpinnerModule } from '@nebular/theme';


@NgModule({
  declarations: [NotFoundComponent],
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
  ],
})
export class SharedModule { }
