import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MarketOverviewComponent } from '../market-overview/market-overview.component';
import { TopPerformersComponent } from '../market-overview/top-performers/top-performers.component';
import { SecuritiesComponent } from '../securities/securities.component';
import { ChartComponent } from '../market-overview/chart/chart.component';

@Component({
  selector: 'app-home',
  imports: [MarketOverviewComponent, TopPerformersComponent, SecuritiesComponent, ChartComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  constructor(private router: Router) { }

  onGetStarted() {
    this.router.navigate(['/register']);
  }
}
