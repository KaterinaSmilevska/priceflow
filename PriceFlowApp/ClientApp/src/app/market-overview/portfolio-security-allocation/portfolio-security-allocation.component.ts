import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartData, ChartOptions, registerables } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartService } from '../chart/chart.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

Chart.register(...registerables, ChartDataLabels);

@Component({
  selector: 'app-portfolio-security-allocation',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, TranslateModule],
  templateUrl: './portfolio-security-allocation.component.html',
  styleUrl: './portfolio-security-allocation.component.css',
})
export class PortfolioSecurityAllocationComponent implements OnInit, OnChanges, OnDestroy {
  @Input() portfolioId!: number;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private langSubscription?: Subscription;

  @Input() isReal: boolean = true;

  public type: 'pie' = 'pie';
  public data: ChartData<'pie', number[], string> = {
    labels: [],
    datasets: [],
  };

  public noDataMessage: string | null = null;

  public options: ChartOptions<'pie'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: {
        display: true,
        text: this.translateService.instant('CHART.SECURITY_ALLOCATION_MESSAGE'),
        font: {
          size: 16,
          weight: 'bold'
        },
        padding: {
          top: 10,
          bottom: 10
        }
      },
      legend: {
        position: 'top'
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

    this.chartService.getSecurityAllocation(this.portfolioId, this.isReal).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = 'NO_HOLDINGS_DATA_AVAILABLE';
          this.chart?.update();
          return;
        }

        const values = res.map(x => x.quantity);
        const colors = this.chartService.generateColors(values.length);

        this.data = {
          labels: res.map(x => x.securityCode),
          datasets: [
            {
              data: values,
              backgroundColor: colors,
            }
          ],
        };
        this.chart?.update();
      },
      error: (err) => {
        console.log(err);
        this.noDataMessage = 'LOADING_DATA_ERROR';
      }
    });
  }
}
