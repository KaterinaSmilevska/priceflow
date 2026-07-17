import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { SecurityFilterService } from './security-filter.service';
import { ChartConfiguration } from 'chart.js';
import { FormsModule } from '@angular/forms';
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { Observable, Subscription } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SectorsTranslatePipe } from '../shared/sectors-translate.pipe';

@Component({
  selector: 'app-security-filter',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, TranslateModule, SectorsTranslatePipe],
  templateUrl: './security-filter.component.html',
  styleUrl: './security-filter.component.css',
})
export class SecurityFilterComponent implements OnInit, OnDestroy {
  data: any[] = [];
  loading = false;
  error = '';
  private _topN = 10;
  selectedMetric = '';

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private langSubscription?: Subscription;

  chartData: ChartConfiguration<'bar'>['data'] = {
    labels: [],
    datasets: [
      {
        label: '',
        data: []
      }
    ]
  };

  chartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    plugins: {
      datalabels: {
        display: (context) => {
          return this.selectedMetric !== 'Market price : Book value';
        }
      },
      tooltip: {
        callbacks: {
          label: (context): string => {
            const value = Number(context.raw);

            const formatted = this.selectedMetric === 'Market price : Book value'
              ? value.toFixed(4)
              : value;

            return formatted.toString();
          }
        }
      }
    }
  };

  constructor(private securityFilterService: SecurityFilterService, private translateService: TranslateService,
    private sectorsTranslatePipe: SectorsTranslatePipe) { }

  ngOnInit(): void {
    this.selectedMetric = 'Market price : Book value';
    this.loadMetric(this.selectedMetric);
    this.langSubscription = this.translateService.onLangChange.subscribe(() => {
      this.prepareChart();
    });
  }

  ngOnDestroy(): void {
    this.langSubscription?.unsubscribe();
  }

  get topN(): number {
    return this._topN;
  }

  set topN(value: number) {
    this._topN = value;
    if (this.selectedMetric) {
      this.loadMetric(this.selectedMetric);
    }
  }

  loadMetric(metric: string) {
    this.loading = true;
    this.error = '';
    this.selectedMetric = metric;

    let request: Observable<any[]>;

    switch (metric) {
      case 'mostProfitableSecuritiesByDividendYield':
        request = this.securityFilterService.getMostProfitableSecuritiesByDividendYield();
        break;
      case 'mostProfitableSecuritiesByDividendPerShare':
        request = this.securityFilterService.getMostProfitableSecuritiesByDividendPerShare();
        break;
      case 'securitiesWithBiggestPriceOscillations':
        request = this.securityFilterService.getSecuritiesWithBiggestPriceOscillations();
        break;
      case 'securitiesWithSmallestPriceOscillations':
        request = this.securityFilterService.getSecuritiesWithSmallestPriceOscillations();
        break;
      case 'mostLiquidSecuritiesByTradedQuantity':
        request = this.securityFilterService.getMostLiquidSecuritiesByTradedQuantity();
        break;
      case 'leastLiquidSecuritiesByNumTradedQuantity':
        request = this.securityFilterService.getLeastLiquidSecuritiesByTradedQuantity();
        break;
      case 'mostLiquidSecuritiesByNumTradingDays':
        request = this.securityFilterService.getMostLiquidSecuritiesByNumTradingDays();
        break;
      case 'leastLiquidSecuritiesByNumTradingDays':
        request = this.securityFilterService.getLeastLiquidSecuritiesByNumTradingDays();
        break;
      case 'mostProfitableSectorsByDividendYield':
        request = this.securityFilterService.getMostProfitableSectorsByDividendYield();
        break;
      case 'mostProfitableSectorsByProfit':
        request = this.securityFilterService.getMostProfitableSectorsByProfit();
        break;
      case 'Market price : Book value':
        request = this.securityFilterService.getSecuritiesValuation();
        break;
      default:
        return;
    }

    request.subscribe({
      next: (res: any[]) => {
        this.data = res.slice(0, this.topN);
        this.prepareChart();
        this.loading = false;
      },
      error: () => {
        this.error = 'LOADING_DATA_ERROR';
        this.loading = false;
      }
    });
  }

  prepareChart() {
    const values = this.data.map(x => x.value ?? x.totalValue);

    this.chartData.labels = this.data.map(x => x.securityCode ||
      this.translateService.instant(this.sectorsTranslatePipe.transform(x.sectorName)));

    this.chartData.datasets[0].data = values;

    this.chartData.datasets[0].label = this.translateService.instant(this.getMetricTranslationKey(this.selectedMetric));

    if (this.selectedMetric === 'Market price : Book value') {
      this.chartData.datasets[0].backgroundColor = values.map(v => {
        if (v > 1.05) return '#dc2626';
        if (v < 0.95) return '#16a34a';
        return '#6b7280';
      });
    } else {
      this.chartData.datasets[0].backgroundColor = '#87ceeb';
    }

    this.chart?.update();
  }

  getMetricTranslationKey(metric: string): string {
    switch (metric) {
      case 'mostProfitableSecuritiesByDividendYield':
        return 'METRIC.MOST_PROFITABLE_SECURITIES_DIVIDEND_YIELD';

      case 'mostProfitableSecuritiesByDividendPerShare':
        return 'METRIC.MOST_PROFITABLE_SECURITIES_DIVIDEND_PER_SHARE';

      case 'securitiesWithBiggestPriceOscillations':
        return 'METRIC.BIGGEST_PRICE_OSCILLATIONS';

      case 'securitiesWithSmallestPriceOscillations':
        return 'METRIC.SMALLEST_PRICE_OSCILLATIONS';

      case 'mostLiquidSecuritiesByTradedQuantity':
        return 'METRIC.MOST_LIQUID_SECURITIES_TRADED_QTY';

      case 'leastLiquidSecuritiesByNumTradedQuantity':
        return 'METRIC.LEAST_LIQUID_SECURITIES_TRADED_QTY';

      case 'mostLiquidSecuritiesByNumTradingDays':
        return 'METRIC.MOST_LIQUID_SECURITIES_TRADING_DAYS';

      case 'leastLiquidSecuritiesByNumTradingDays':
        return 'METRIC.LEAST_LIQUID_SECURITIES_TRADING_DAYS';

      case 'mostProfitableSectorsByDividendYield':
        return 'METRIC.MOST_PROFITABLE_SECTORS_DIVIDEND_YIELD';

      case 'mostProfitableSectorsByProfit':
        return 'METRIC.MOST_PROFITABLE_SECTORS_PROFIT';

      case 'Market price : Book value':
        return 'METRIC.SECURITIES_VALUATION';

      default:
        return metric;
    }
  }

  getValuationClass(item: any): string {
    if (this.selectedMetric !== 'Market price : Book value') {
      return '';
    }
    const value = item.value || item.totalValue;

    if (value > 1.05) {
      return 'overvalued';
    }
    if (value < 0.95) {
      return 'undervalued';
    }
    return 'neutral';
  }
}
