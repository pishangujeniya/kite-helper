import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { AccountComponent } from './account.component';
import { LoginComponent } from './login/login.component';
import {
  NbAlertModule,
  NbButtonModule,
  NbCardModule,
  NbCheckboxModule,
  NbIconModule,
  NbInputModule,
  NbLayoutModule,
  NbMenuModule,
  NbSpinnerModule,
  NbToastrModule,
  NbTooltipModule,
} from '@nebular/theme';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NbAuthModule } from '@nebular/auth';
import { LogoutComponent } from './logout/logout.component';

@NgModule({
  declarations: [
    AccountComponent,
    LoginComponent,
    LogoutComponent,
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    NbSpinnerModule,
    NbMenuModule,
    NbLayoutModule,
    NbCardModule,
    NbCheckboxModule,
    NbAlertModule,
    NbInputModule,
    NbButtonModule,
    RouterModule,
    FormsModule,
    NbIconModule,
    NbAuthModule,
    NbTooltipModule,
    NbToastrModule,
  ],
})
export class AccountModule { }
