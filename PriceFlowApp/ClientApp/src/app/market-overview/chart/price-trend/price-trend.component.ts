import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartConfiguration, ChartData, ChartType, registerables } from 'chart.js';
import { ChartService } from '../chart.service';
import { BaseChartDirective } from 'ng2-charts';

Chart.register(...registerables);

@Component({
  selector: 'app-price-trend',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, FormsModule],
  templateUrl: './price-trend.component.html',
})

export class PriceTrendComponent implements OnInit {
  @Input() securityId!: number;
  @Input() startDate!: string;
  @Input() endDate!: string;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public type: ChartType = 'line';
  public data: ChartData<'line'> = {
    labels: [],
    datasets: [],
  };

  public securities: { id: number, code: string }[] = [];
  public noDataMessage: string | null = null;

  constructor(private chartService: ChartService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      (changes['securityId'] && !changes['securityId'].firstChange) ||
      (changes['startDate'] && !changes['startDate'].firstChange) ||
      (changes['endDate'] && !changes['endDate'].firstChange)
    ) {
      this.loadChart();
    }
  }

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
          this.noDataMessage = "No available trading data.";
        },
      });
    }
    else {
      this.loadChart();
    }

    //if (this.securityId && this.startDate && this.endDate) {
    //  this.chartService.getSecurities().subscribe(security => {
    //    this.securities = security.map(s => ({ id: s.id, code: s.code }));
    //    if (this.securities.length > 0) {
    //      this.securityId = this.securities[0].id;
    //      const today = new Date();
    //      this.startDate = this.chartService.formatDate(new Date(today.getFullYear(), today.getMonth() - 3, today.getDate()));
    //      this.endDate = this.chartService.formatDate(today);
    //      this.loadChart();
    //    }
    //  });
    //}
  }

  loadChart(): void {
    this.noDataMessage = null;
    if (!this.securityId || !this.startDate || !this.endDate) return;

    this.chartService.getPriceTrend(this.securityId, this.startDate, this.endDate).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = `No data available for this period.`;
          this.chart?.update();
          return;
        }
        this.data = {
          labels: res.map((d) => new Date(d.date).toLocaleDateString()),
          datasets: [
            {
              label: 'Price',
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
        this.noDataMessage = "Error loading price trend data.";
      },
      
    });
  }

  onFilterChange(): void {
    this.loadChart();
  }
}
