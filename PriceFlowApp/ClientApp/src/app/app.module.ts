//import { BrowserModule } from '@angular/platform-browser';
//import { NgModule } from '@angular/core';
//import { FormsModule } from '@angular/forms';
//import { RouterModule, Routes } from '@angular/router';
//import { HttpClientModule } from '@angular/common/http';
//import { ReactiveFormsModule } from '@angular/forms';

//import { AppComponent } from './app.component';
//import { HomeComponent } from './home/home.component';
//import { RegisterComponent } from './auth/register/register.component';
//import { BrokerComponent } from './broker/broker.component';
//import { LoginComponent } from './auth/login/login.component';
//import { AdminComponent } from './admin/admin.component';
//import { EditUserComponent } from './admin/edit-user.component';
//import { AuthGuard } from './auth/auth.guard';
//import { VerifyEmailComponent } from './auth/register/verify-email.component';
//import { ResetPasswordComponent } from './reset-password/reset-password.component';
//import { SecuritiesComponent } from './securities/securities.component';
//import { AddSecurityComponent } from './securities/add-security/add-security.component';
//import { MarketOverviewComponent } from './market-overview/market-overview.component';

//const routes: Routes = [
//  {
//    path: '', component: HomeComponent, pathMatch: 'full'
//  },
//  { path: 'register', component: RegisterComponent },
//  { path: 'login', component: LoginComponent },
//  { path: 'broker', component: BrokerComponent, canActivate: [AuthGuard] },
//  { path: 'admin/users', component: AdminComponent, canActivate: [AuthGuard] },
//  { path: 'admin/users/:id', component: EditUserComponent, canActivate: [AuthGuard]},
//  { path: 'verify-email', component: VerifyEmailComponent, canActivate: [AuthGuard] },
//  { path: 'forgot-password', component: ResetPasswordComponent },
//  { path: 'securities', component: SecuritiesComponent },
//  { path: 'securities/add', component: AddSecurityComponent, canActivate: [AuthGuard] },
//  { path: 'marketoverview', component: MarketOverviewComponent},
//]

//@NgModule({
//  declarations: [
//  ],
//  imports: [
//    BrowserModule,
//    HttpClientModule,
//    FormsModule,
//    RouterModule.forRoot(routes, { useHash: false }),
//    ReactiveFormsModule,
//  ],
//  exports: [RouterModule],
//  providers: [AuthGuard],
//  bootstrap: [AppComponent]
//})
//export class AppModule { }
