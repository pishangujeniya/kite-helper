import { NgModule } from '@angular/core';
import { NbCardModule, NbIconModule, NbMenuModule, NbSpinnerModule } from '@nebular/theme';

import { ThemeModule } from '../@theme/theme.module';
import { DashboardComponent } from './dashboard.component';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { SharedModule } from './shared/shared.module';


@NgModule({
  imports: [
    DashboardRoutingModule,
    ThemeModule,
    NbMenuModule,
    SharedModule,
    NbCardModule,
    NbIconModule,
    NbSpinnerModule,
    SharedModule,
  ],
  declarations: [
    DashboardComponent,
  ],
})
export class DashboardModule {
}
