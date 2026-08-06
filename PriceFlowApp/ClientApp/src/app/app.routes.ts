import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './auth/register/register.component';
import { LoginComponent } from './auth/login/login.component';
import { BrokersComponent } from './brokers/brokers.component';
import { UserFormComponent } from './admin/manage-users/user-form/user-form.component';
import { VerifyEmailComponent } from './auth/register/verify-email.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { SecuritiesComponent } from './securities/securities.component';
import { SecurityFormComponent } from './securities/security-form/security-form.component';
import { MarketOverviewComponent } from './market-overview/market-overview.component';
import { AuthGuard } from './auth/auth.guard';
import { PortfoliosComponent } from './portfolios/portfolios.component';
import { InvestorGuard } from './portfolios/investor.guard';
import { PortfolioDetailsComponent } from './portfolios/portfolio-details/portfolio-details.component';
import { ThresholdComponent } from './thresholds/threshold.component';
import { ManageUsersComponent } from './admin/manage-users/manage-users.component';
import { ManageBrokersComponent } from './admin/manage-brokers/manage-brokers.component';
import { SecurityFilterComponent } from './security-filter/security-filter.component';
import { AnalystGuard } from './security-filter/analyst.guard';
import { AdminGuard } from './admin/admin.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  { path: 'brokers', component: BrokersComponent, canActivate: [InvestorGuard] },
  { path: 'admin/brokers', component: ManageBrokersComponent, canActivate: [AdminGuard] },
  { path: 'admin/users', component: ManageUsersComponent, canActivate: [AdminGuard] },
  { path: 'admin/users/:id', component: UserFormComponent, canActivate: [AdminGuard] },
  { path: 'verify-email', component: VerifyEmailComponent, canActivate: [AuthGuard] },
  { path: 'forgot-password', component: ResetPasswordComponent },
  { path: 'securities', component: SecuritiesComponent },
  { path: 'securities/add', component: SecurityFormComponent, canActivate: [AuthGuard] },
  { path: 'marketoverview', component: MarketOverviewComponent },
  { path: 'portfolios', component: PortfoliosComponent, canActivate: [InvestorGuard] },
  { path: 'portfolios/:id', component: PortfolioDetailsComponent, canActivate: [InvestorGuard] },
  { path: 'price-alerts', component: ThresholdComponent, canActivate: [InvestorGuard] },
  { path: 'security-filter', component: SecurityFilterComponent, canActivate: [AnalystGuard] },
  { path: '**', redirectTo: '' },
];
