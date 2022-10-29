import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { UsersComponent } from './users.component';
import { NbButtonModule, NbCardModule, NbCheckboxModule, NbDatepickerModule, NbIconModule, NbInputModule, NbRadioModule, NbSelectModule, NbSpinnerModule, NbStepperModule, NbTabsetModule } from '@nebular/theme';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { SharedModule } from '../shared/shared.module';


@NgModule({
  declarations: [
    UsersComponent,
    UserProfileComponent,
  ],
  imports: [
    CommonModule,
    UsersRoutingModule,
    NbIconModule,
    CommonModule,
    NbIconModule,
    NbCardModule,
    FormsModule,
    ReactiveFormsModule,
    NbInputModule,
    NbRadioModule,
    NbSelectModule,
    NbButtonModule,
    NbSpinnerModule,
    NbDatepickerModule,
    NbStepperModule,
    NbTabsetModule,
    NbDatepickerModule,
    NbCheckboxModule,
    SharedModule,
  ],
})
export class UsersModule { }
