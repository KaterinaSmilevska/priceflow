import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { MarketOverviewComponent } from '../market-overview/market-overview.component';
import { TopPerformersComponent } from '../market-overview/top-performers/top-performers.component';
import { ChartComponent } from '../market-overview/chart/chart.component';
import { BrokersComponent } from '../brokers/brokers.component';
import { CommonModule } from '@angular/common';
import { LoginService } from '../auth/login/login.service';
import { SecuritiesPriceTrendComponent } from '../portfolios/securities-price-trend/securities-price-trend.component';
import { LiquidityComponent } from '../liquidity/liquidity.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule, BrokersComponent, MarketOverviewComponent, TopPerformersComponent,
    ChartComponent, SecuritiesPriceTrendComponent, LiquidityComponent, TranslateModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  portfolioId?: number;
  constructor(private router: Router, public loginService: LoginService) { }

  onGetStarted() {
    this.router.navigate(['/register']);
  }
}
