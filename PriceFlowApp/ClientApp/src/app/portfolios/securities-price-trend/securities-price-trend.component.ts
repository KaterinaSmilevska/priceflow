import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { PortfoliosService, SecuritiesPriceTrend } from '../portfolios.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-securities-price-trend',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, TranslateModule],
  templateUrl: './securities-price-trend.component.html',
  styleUrl: './securities-price-trend.component.css',
})
export class SecuritiesPriceTrendComponent implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  data: SecuritiesPriceTrend[] = [];
  securities: string[] = [];

  selectedSecurity?: string;

  selectedPeriod: 'Monthly' | 'Yearly' = 'Monthly';

  chartData: ChartData<'line'> = {
    labels: [],
    datasets: []
  };

  chartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
      },
      datalabels: {
        display: false
      }
    },
    scales: {
      x: {
        title: {
          display: true,
          text: 'Date',
          font: {
            weight: 'bold'
          }
        },
        ticks: {
          maxRotation: 0,
          autoSkip: true,
          maxTicksLimit: 10
        }
      },
      y: {
        title: {
          display: true,
          text: 'Price (MKD)',
          font: {
            weight: 'bold'
          }
        },
        beginAtZero: false
      }
    }
  };

  constructor(private portfoliosService: PortfoliosService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.portfoliosService
      .getSecuritiesPriceTrend(this.selectedPeriod)
      .subscribe(res => {
        this.data = res;
        this.securities = [...new Set(res.map(r => r.securityCode))];
        this.buildChart();
      });
  }

  onSecurityChange(): void {
    this.loadData();
  }

  private buildChart(): void {
    const filtered = this.selectedSecurity ? this.data.filter(r => r.securityCode === this.selectedSecurity)
      : this.data;

    const grouped = new Map<string, SecuritiesPriceTrend[]>();

    filtered.forEach(r => {
      if (!grouped.has(r.securityCode)) {
        grouped.set(r.securityCode, []);
      }
      grouped.get(r.securityCode)!.push(r);
    });

    const allDates = [...new Set(filtered.map(d => d.date))]
      .sort((a, b) => new Date(a).getTime() - new Date(b).getTime());

    this.chartData = {
      labels: allDates,
      datasets: Array.from(grouped.entries()).map(([code, values], idx) => ({
        label: code,
        data: allDates.map(date => {
          const v = values.find(d => d.date === date);
          return v ? v.price : null;
        }),
        tension: 0.35,
        borderWidth: 3,
        pointRadius: 0,
        borderColor: `hsl(${(idx * 160) % 360}, 85%, 55%)`,
        backgroundColor: `hsla(${(idx * 160) % 360}, 85%, 55%, 0.25)`
      }))
    };
    this.chart?.update();
  }
}
