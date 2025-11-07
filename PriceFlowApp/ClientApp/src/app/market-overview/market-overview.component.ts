import { Component, OnInit } from '@angular/core';
import { MarketOverview, MarketOverviewService } from './market-overview.service';

@Component({
  selector: 'app-market-overview',
  templateUrl: './market-overview.component.html',
  styleUrls: ['./market-overview.component.css']
})
export class MarketOverviewComponent implements OnInit {
  overview?: MarketOverview;
  loading = true;

  constructor(private marketOverviewService: MarketOverviewService) { }

  ngOnInit(): void {
    this.marketOverviewService.getOverview().subscribe({
      next: data => {
        this.overview = data;
        this.loading = false;
      },
      error: err => {
        console.error(err);
        this.loading = false;
      }
    });
  }
}
