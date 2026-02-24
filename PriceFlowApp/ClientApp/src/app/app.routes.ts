import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { BrokersComponent } from './brokers/brokers.component';
import { AdminComponent } from './admin/admin.component';
import { EditUserComponent } from './admin/edit-user.component';
import { VerifyEmailComponent } from './auth/register/verify-email.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { SecuritiesComponent } from './securities/securities.component';
import { AddSecurityComponent } from './securities/add-security/add-security.component';
import { MarketOverviewComponent } from './market-overview/market-overview.component';
import { TopPerformersComponent } from './market-overview/top-performers/top-performers.component';
import { AuthGuard } from './auth/auth.guard';
import { PortfoliosComponent } from './portfolios/portfolios.component';
import { InvestorGuard } from './portfolios/investor.guard';
import { PortfolioDetailsComponent } from './portfolios/portfolio-details/portfolio-details.component';
import { ThresholdComponent } from './thresholds/threshold.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'brokers', component: BrokersComponent, canActivate: [InvestorGuard] },
  { path: 'admin/users', component: AdminComponent, canActivate: [AuthGuard] },
  { path: 'admin/users/:id', component: EditUserComponent, canActivate: [AuthGuard] },
  { path: 'verify-email', component: VerifyEmailComponent, canActivate: [AuthGuard] },
  { path: 'forgot-password', component: ResetPasswordComponent },
  { path: 'securities', component: SecuritiesComponent },
  { path: 'securities/add', component: AddSecurityComponent, canActivate: [AuthGuard] },
  { path: 'marketoverview', component: MarketOverviewComponent },
  { path: 'portfolios', component: PortfoliosComponent, canActivate: [InvestorGuard] },
  { path: 'portfolios/:id', component: PortfolioDetailsComponent, canActivate: [InvestorGuard] },
  { path: 'price-alerts', component: ThresholdComponent, canActivate: [InvestorGuard] },
  { path: '**', redirectTo: '' },
];
