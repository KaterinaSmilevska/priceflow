import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { PortfoliosService } from '../portfolios.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SecuritiesPriceTrend } from '../SecuritiesPriceTrend';
import { Subscription } from 'rxjs';
import { SecurityPriceTrendReport } from './SecurityPriceTrendReport';

@Component({
  selector: 'app-securities-price-trend',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, TranslateModule],
  templateUrl: './securities-price-trend.component.html',
  styleUrl: './securities-price-trend.component.css',
})
export class SecuritiesPriceTrendComponent implements OnInit, OnDestroy {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private langSubscription?: Subscription;

  data: SecuritiesPriceTrend[] = [];
  securities: string[] = [];

  selectedSecurity?: string;

  selectedPeriod: 'Monthly' | 'Yearly' = 'Monthly';

  selectedResolution: 'Day' | 'Week' | 'Month' | 'Quarter' = 'Week';
  availableResolutions: {
    value: 'Day' | 'Week' | 'Month' | 'Quarter',
    label: string
  }[] = [];

  reports: SecurityPriceTrendReport[] = [];

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
      },
      zoom: {
        pan: {
          enabled: false
        },
        zoom: {
          wheel: {
            enabled: true
          },
          pinch: {
            enabled: false
          },
          drag: {
            enabled: false
          },
          mode: 'x'
        }
      }
    },
    scales: {
      x: {
        title: {
          display: true,
          text: this.translateService.instant('DATE'),
          font: {
            weight: 'bold'
          }
        },
        ticks: {
          autoSkip: this.selectedResolution !== 'Day',
          maxTicksLimit: this.selectedResolution === 'Month' ? 12 :
            this.selectedResolution === 'Quarter' ? 4 : undefined,
          minRotation: this.selectedResolution === 'Day' ? 45 : 0,
          maxRotation: this.selectedResolution === 'Day' ? 45 : 0
        }
      },
      y: {
        title: {
          display: true,
          text: `${this.translateService.instant('PRICE')} (${this.translateService.instant('CURRENCY.MKD')})`,
          font: {
            weight: 'bold'
          }
        },
        beginAtZero: false,
      }
    }
  };

  constructor(private portfoliosService: PortfoliosService, private translateService: TranslateService) { }

  ngOnInit(): void {
    this.updateAvailableResolutions();
    this.loadData();

    this.langSubscription = this.translateService.onLangChange.subscribe(() => {
      this.updateChartOptions();
      this.updateAvailableResolutions();
      this.buildChart();
      this.chart?.update();
    });
  }

  ngOnDestroy(): void {
    this.langSubscription?.unsubscribe();
  }

  loadData(): void {
    this.portfoliosService
      .getSecuritiesPriceTrend(this.selectedPeriod, this.selectedResolution)
      .subscribe(res => {
        this.data = res;
        this.securities = [...new Set(res.map(r => r.securityCode))];
        this.buildChart();
        this.loadReport();
      });
  }

  onSecurityChange(): void {
    this.selectedResolution = this.determineResolution();
    this.refresh();
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

    const dateLabels = allDates.map(date => {
      const d = new Date(date);

      return this.formatLabel(new Date(d));
    });

    this.chartData = {
      labels: dateLabels,
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

  private updateChartOptions(): void {
    this.chartOptions = {
      ...this.chartOptions,
      scales: {
        ...this.chartOptions.scales,
        x: {
          ...this.chartOptions.scales!.x,
          title: {
            ...this.chartOptions.scales!.x!.title,
            display: true,
            text: this.translateService.instant('DATE')
          },
          ticks: {
            autoSkip: this.selectedResolution !== 'Day',
            maxTicksLimit: this.selectedResolution === 'Month' ? 12 :
              this.selectedResolution === 'Quarter' ? 4 : undefined,
            minRotation: this.selectedResolution === 'Day' ? 45 : 0,
            maxRotation: this.selectedResolution === 'Day' ? 45 : 0
          }
        },
        y: {
          ...this.chartOptions.scales!.y,
          title: {
            ...this.chartOptions.scales!.y!.title,
            display: true,
            text: `${this.translateService.instant('PRICE')} (${this.translateService.instant('CURRENCY.MKD')})`,
          }
        }
      }
    };
  }

  private updateAvailableResolutions(): void {
    if (this.selectedPeriod === 'Monthly') {
      this.availableResolutions = [
        { value: 'Day', label: this.translateService.instant('DAILY') },
        { value: 'Week', label: this.translateService.instant('WEEKLY') }
      ];
    } else {
      this.availableResolutions = [
        { value: 'Day', label: this.translateService.instant('DAILY') },
        { value: 'Week', label: this.translateService.instant('WEEKLY') },
        { value: 'Month', label: this.translateService.instant('MONTHLY') },
        { value: 'Quarter', label: this.translateService.instant('QUARTERLY') }
      ];
    }
  }

  private determineResolution(): 'Day' | 'Week' | 'Month' | 'Quarter' {
    const numberOfSecurities = this.selectedSecurity ? 1 : this.securities.length;

    if (this.selectedPeriod === 'Monthly') {
      if (numberOfSecurities <= 3) {
        return 'Day';
      } else {
        return 'Week';
      }
    }
    else {
      if (numberOfSecurities <= 5) {
        return 'Month';
      } else {
        return 'Quarter';
      }
    }
  }

  onPeriodChange() {
    this.updateAvailableResolutions();
    this.selectedResolution = this.determineResolution();
    this.updateChartOptions();
    this.refresh();
  }

  onResolutionChange() {
    this.refresh();
  }

  private loadReport(): void {
    this.portfoliosService
      .getSecuritiesPriceTrendReport(this.selectedPeriod, this.selectedResolution, this.selectedSecurity)
      .subscribe(report => {
        this.reports = report;
      });
  }

  private refresh(): void {
    this.loadData();
    this.loadReport();
  }

  resetZoom(): void {
    this.chart?.chart?.resetZoom();
  }

  private formatLabel(date: Date): string {
    switch (this.selectedResolution) {
      case 'Day':
        return date.toLocaleDateString('en-GB', {
          day: '2-digit',
          month: '2-digit',
          year: 'numeric'
        });

      case 'Week':
        return date.toLocaleDateString('en-GB', {
          day: '2-digit',
          month: 'short'
        });

      case 'Month':
        return date.toLocaleDateString('en-GB', {
          month: 'short',
          year: 'numeric'
        });

      case 'Quarter': {
        const quarter = Math.floor(date.getMonth() / 3) + 1;
        return `Q${quarter} ${date.getFullYear()}`;
      }
    }
  }

  downloadPDF() {
    this.portfoliosService
      .generateSecuritiesPriceTrendReport(this.selectedPeriod, this.selectedResolution, this.selectedSecurity)
      .subscribe(blob => {
        const fileURL = URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.href = fileURL;
        link.download = 'SecuritiesPriceTrendReport.pdf';

        link.click();

        URL.revokeObjectURL(fileURL);
      });
  }
}
