import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { MarketOverviewComponent } from '../market-overview/market-overview.component';
import { TopPerformersComponent } from '../market-overview/top-performers/top-performers.component';
import { SecuritiesComponent } from '../securities/securities.component';
import { ChartComponent } from '../market-overview/chart/chart.component';
import { BrokersComponent } from '../brokers/brokers.component';
import { CommonModule } from '@angular/common';
import { LoginService } from '../auth/login/login.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule, BrokersComponent, MarketOverviewComponent, TopPerformersComponent, SecuritiesComponent, ChartComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  constructor(private router: Router, public loginService: LoginService) { }

  onGetStarted() {
    this.router.navigate(['/register']);
  }
}
