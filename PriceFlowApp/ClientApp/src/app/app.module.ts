import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { BrokerComponent } from './broker/broker.component';
import { LoginComponent } from './auth/login/login.component';
import { AdminComponent } from './admin/admin.component';
import { EditUserComponent } from './admin/edit-user.component';
import { AuthGuard } from './auth/auth.guard';
import { VerifyEmailComponent } from './auth/register/verify-email.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { SecuritiesComponent } from './securities/securities.component';

const routes: Routes = [
  {
    path: '', redirectTo: '', component: HomeComponent, pathMatch: 'full'
  },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'broker', component: BrokerComponent, canActivate: [AuthGuard] },
  { path: 'admin/users', component: AdminComponent, canActivate: [AuthGuard] },
  { path: 'admin/users/:id', component: EditUserComponent, canActivate: [AuthGuard]},
  { path: 'verify-email', component: VerifyEmailComponent, canActivate: [AuthGuard] },
  { path: 'forgot-password', component: ResetPasswordComponent },
  { path: 'securities', component: SecuritiesComponent},
]

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    RegisterComponent,
    LoginComponent,
    BrokerComponent,
    ResetPasswordComponent,
    AdminComponent,
    EditUserComponent,
    SecuritiesComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(routes, {useHash: false})
  ],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
