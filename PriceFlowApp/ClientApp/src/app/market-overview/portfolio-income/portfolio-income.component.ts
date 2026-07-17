import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartData, ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartService } from '../chart/chart.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-portfolio-income',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, TranslateModule],
  templateUrl: './portfolio-income.component.html',
  styleUrl: './portfolio-income.component.css',
})
export class PortfolioIncomeComponent implements OnInit, OnChanges, OnDestroy {
  @Input() portfolioId!: number;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private langSubscription?: Subscription;

  @Input() isReal: boolean = true;

  public type: 'line' = 'line';
  public data: ChartData<'line', number[], string> = {
    labels: [],
    datasets: [],
  };

  public noDataMessage: string | null = null;

  options: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: {
        display: true,
        text: this.translateService.instant('PORTFOLIOS.PORTFOLIO_INCOME'),
        font: {
          size: 16,
          weight: 'bold'
        },
        padding: {
          top: 10,
          bottom: 20
        },
      },
      legend: {
        display: true
      }
    }
  };

  constructor(private chartService: ChartService, private translateService: TranslateService) { }

  ngOnInit(): void {
    this.loadChart();
    this.langSubscription = this.translateService.onLangChange.subscribe(() => {
      this.loadChart();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['portfolioId'] && !changes['portfolioId'].firstChange) {
      this.loadChart();
    }
    if (changes['isReal'] && !changes['isReal'].firstChange) {
      this.loadChart();
    }
  }

  ngOnDestroy(): void {
    this.langSubscription?.unsubscribe();
  }

  loadChart(): void {
    this.noDataMessage = null;

    this.chartService.getPortfolioIncome(this.portfolioId, this.isReal).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = 'NO_TRANSACTIONS_DATA_AVAILABLE';
          this.chart?.update();
          return;
        }

        const labels = res.map(x => `${x.month}.${x.year}`);
        this.data = {
          labels,
          datasets: [
            {
              label: this.translateService.instant('PORTFOLIOS.INCOME'),
              data: res.map(x => x.income),
              borderColor: '#007bff',
              backgroundColor: 'rgba(0,123,255,0.2)',
              fill: true,
              tension: 0.3,
            }
          ],
        };

        (this.options as any).plugins.title.text = this.translateService.instant(
          'CHART.PORTFOLIO_INCOME_MESSAGE',
          { start: labels[0], end: labels[labels.length - 1] }
        );
        this.chart?.update();
      },
      error: (err) => {
        console.log(err);
        this.noDataMessage = 'LOADING_DATA_ERROR';
      }
    });
  }
}
