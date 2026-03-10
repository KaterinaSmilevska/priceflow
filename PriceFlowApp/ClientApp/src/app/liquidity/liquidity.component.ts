import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MarketOverviewService } from '../market-overview/market-overview.service';
import { FormsModule } from '@angular/forms';
import { LiquidityOverview } from './LiquidityOverview';
import { LiquidityTableComponent } from './liquidity-table/liquidity-table.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-liquidity',
  standalone: true,
  imports: [CommonModule, FormsModule, LiquidityTableComponent, TranslateModule],
  templateUrl: './liquidity.component.html',
  styleUrl: './liquidity.component.css',
})
export class LiquidityComponent {
  liquidity: LiquidityOverview = {
    mostByTradedQuantity: [],
    leastByTradedQuantity: [],
    mostByTradingDays: [],
    leastByTradingDays: []
  };
  months = 6;

  noDataMessage: string | null = null;

  constructor(private marketOverviewService: MarketOverviewService) { }

  ngOnInit() {
    this.loadLiquidity();
  }

  loadLiquidity() {
    this.noDataMessage = null;
    this.marketOverviewService.getLiquidity(this.months)
      .subscribe({
        next: (res) => {
          if (!res) {
            this.noDataMessage = 'NO_DATA_AVAILABLE_FOR_PERIOD'
            return;
          }
          this.liquidity = res;
        },
        error: (err) => {
          console.error(err);
          this.noDataMessage = 'LOADING_DATA_ERROR'
        }
      });
  }
}
