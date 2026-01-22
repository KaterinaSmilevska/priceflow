import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartType, ChartData } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartService } from '../chart/chart.service';

@Component({
  selector: 'app-portfolio-income',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './portfolio-income.component.html',
  styleUrl: './portfolio-income.component.css',
})
export class PortfolioIncomeComponent implements OnInit {
  @Input() portfolioId!: number;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public type: ChartType = 'line';
  public data: ChartData<'line', number[], string> = {
    labels: [],
    datasets: [],
  };

  public noDataMessage: string | null = null;

  constructor(private chartService: ChartService) { }

  ngOnInit(): void {
    this.loadChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['portfolioId'] && !changes['portfolioId'].firstChange) {
      this.loadChart();
    }
  }

  loadChart(): void {
    this.noDataMessage = null;

    this.chartService.getPortfolioIncome(this.portfolioId).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = "No transactions found.";
          this.chart?.update();
          return;
        }

        this.data = {
          labels: res.map(x => `${x.month}.${x.year}`),
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
        this.chart?.update();
      },
      error: (err) => {
        console.log(err);
        this.noDataMessage = "An error occured while loading data.";
      }
    });
  }
}
