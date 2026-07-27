import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartData, ChartType, registerables } from 'chart.js';
import { ChartService } from '../chart.service';
import { BaseChartDirective } from 'ng2-charts';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';

Chart.register(...registerables);

@Component({
  selector: 'app-price-trend',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, FormsModule, TranslateModule],
  templateUrl: './price-trend.component.html',
})

export class PriceTrendComponent implements OnInit, OnChanges, OnDestroy {
  @Input() securityId!: number;
  @Input() startDate!: string;
  @Input() endDate!: string;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private langSubscription?: Subscription;

  public type: ChartType = 'line';
  public data: ChartData<'line'> = {
    labels: [],
    datasets: [],
  };

  public securities: { id: number, code: string }[] = [];
  public noDataMessage: string | null = null;

  constructor(private chartService: ChartService, private translateService: TranslateService) { }

  ngOnInit(): void {
    if (!this.endDate) {
      this.chartService.getLatestDate().subscribe({
        next: (latestDate) => {
          this.endDate = latestDate;
          const latest = new Date(latestDate);
          const threeMonthsBefore = new Date(latest);
          threeMonthsBefore.setMonth(latest.getMonth() - 3);
          this.startDate = this.chartService.formatDate(threeMonthsBefore);
          this.loadChart();
        },
        error: () => {
          this.noDataMessage = 'NO_TRADING_DATA_AVAILABLE';
        },
      });
    }
    else {
      this.loadChart();
    }
    this.langSubscription = this.translateService.onLangChange.subscribe(() => {
      this.loadChart();
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      (changes['securityId'] && !changes['securityId'].firstChange) ||
      (changes['startDate'] && !changes['startDate'].firstChange) ||
      (changes['endDate'] && !changes['endDate'].firstChange)
    ) {
      this.loadChart();
    }
  }

  ngOnDestroy(): void {
    this.langSubscription?.unsubscribe();
  }

  loadChart(): void {
    this.noDataMessage = null;
    if (!this.securityId || !this.startDate || !this.endDate) return;

    this.chartService.getPriceTrend(this.securityId, this.startDate, this.endDate).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = 'NO_DATA_AVAILABLE_FOR_PERIOD';
          this.chart?.update();
          return;
        }
        this.data = {
          labels: res.map((d) => new Date(d.date).toLocaleDateString()),
          datasets: [
            {
              label: this.translateService.instant('PRICE'),
              data: res.map((d) => d.price),
              borderColor: 'green',
              backgroundColor: 'rgba(123, 182, 98, 0.7)',
              tension: 0.2,
              fill: true,
              datalabels: { display: false },
            },
          ],
        };
        this.chart?.update();
      },
      error: (err) => {
        console.error(err);
        this.noDataMessage = "LOADING_DATA_ERROR";
      },
      
    });
  }

  onFilterChange(): void {
    this.loadChart();
  }
}
