import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartData, ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartService } from '../chart/chart.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-portfolio-income',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, TranslateModule],
  templateUrl: './portfolio-income.component.html',
  styleUrl: './portfolio-income.component.css',
})
export class PortfolioIncomeComponent implements OnInit {
  @Input() portfolioId!: number;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  @Input() isReal: boolean = true;

  public type: 'line' = 'line';
  public data: ChartData<'line', number[], string> = {
    labels: [],
    datasets: [],
  };

  public noDataMessage: string | null = null;

  public options: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      title: {
        display: true,
        text: 'Portfolio income over time',
        font: {
          size: 16,
          weight: 'bold'
        },
        padding: {
          top: 10,
          bottom: 20
        }
      },
      legend: {
        display: true
      }
    }
  };

  constructor(private chartService: ChartService) { }

  ngOnInit(): void {
    this.loadChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['portfolioId'] && !changes['portfolioId'].firstChange) {
      this.loadChart();
    }
    if (changes['isReal'] && !changes['isReal'].firstChange) {
      this.loadChart();
    }
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
              label: 'Income',
              data: res.map(x => x.income),
              borderColor: '#007bff',
              backgroundColor: 'rgba(0,123,255,0.2)',
              fill: true,
              tension: 0.3,
            }
          ],
        };

        (this.options as any).plugins.title.text =
          `Portfolio income (${labels[0]} - ${labels[labels.length - 1]})`;
        this.chart?.update();
      },
      error: (err) => {
        console.log(err);
        this.noDataMessage = 'LOADING_DATA_ERROR';
      }
    });
  }
}
