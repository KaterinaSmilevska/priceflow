import { Component, OnInit } from '@angular/core';
import { MarketOverviewService } from './market-overview.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { MarketOverview } from './MarketOverview';

@Component({
  selector: 'app-market-overview',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
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
