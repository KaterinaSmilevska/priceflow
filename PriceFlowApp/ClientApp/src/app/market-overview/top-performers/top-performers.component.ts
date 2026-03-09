import { Component, OnInit } from '@angular/core';
import { MarketOverviewService, SecurityPerformance } from '../market-overview.service';
import { forkJoin } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-top-performers',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './top-performers.component.html',
  styleUrls: ['./top-performers.component.css']
})

export class TopPerformersComponent implements OnInit {
  topGainers: SecurityPerformance[] = [];
  topLosers: SecurityPerformance[] = [];
  mostTraded: SecurityPerformance[] = [];
  loading = true;
  count = 5;

  constructor(private marketOverviewService: MarketOverviewService) { }

  ngOnInit() {
    forkJoin({
      gainers: this.marketOverviewService.getTopGainers(this.count),
      losers: this.marketOverviewService.getTopLosers(this.count),
      traded: this.marketOverviewService.getMostTraded(this.count),
    }).subscribe({
      next: ({ gainers, losers, traded }) => {
        this.topGainers = gainers ?? [];
        this.topLosers = losers ?? [];
        this.mostTraded = traded ?? [];
      },
      error: (err) => console.error('Error loading top performers', err),
      complete: () => (this.loading = false)
    });
  }
}
