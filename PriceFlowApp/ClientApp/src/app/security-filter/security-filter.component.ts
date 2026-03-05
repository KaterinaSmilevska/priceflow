import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { SecurityFilterService } from './security-filter.service';
import { ChartConfiguration } from 'chart.js';
import { FormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-security-filter',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './security-filter.component.html',
  styleUrl: './security-filter.component.css',
})
export class SecurityFilterComponent implements OnInit {
  data: any[] = [];
  loading = false;
  error = '';
  private _topN = 10;
  selectedMetric = '';

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

  constructor(private securityFilterService: SecurityFilterService) { }

  ngOnInit(): void {
    this.selectedMetric = 'Market price : Book value';
    this.loadMetric(this.selectedMetric);
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
        this.error = 'Failed to load data.';
        this.loading = false;
      }
    });
  }

  prepareChart() {
    const values = this.data.map(x => x.value ?? x.totalValue);

    this.chartData.labels = this.data.map(x => x.securityCode || x.sectorName);

    this.chartData.datasets[0].data = values;

    this.chartData.datasets[0].label = this.selectedMetric;

    if (this.selectedMetric === 'Market price : Book value') {
      this.chartData.datasets[0].backgroundColor = values.map(v => {
        if (v > 1) return '#dc2626';
        if (v < 1) return '#16a34a';
        return '#6b7280';
      });
    } else {
      this.chartData.datasets[0].backgroundColor = '#87ceeb';
    }
  }

  getValuationClass(item: any): string {
    if (this.selectedMetric !== 'Market price : Book value') {
      return '';
    }
    const value = item.value || item.totalValue;

    if (value > 1) {
      return 'overvalued';
    }
    if (value < 1) {
      return 'undervalued';
    }
    return 'neutral';
  }
}
