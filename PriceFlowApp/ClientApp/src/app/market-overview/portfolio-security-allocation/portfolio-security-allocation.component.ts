import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, SimpleChange, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartData, ChartType, registerables } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartService } from '../chart/chart.service';

Chart.register(...registerables);

@Component({
  selector: 'app-portfolio-security-allocation',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './portfolio-security-allocation.component.html',
  styleUrl: './portfolio-security-allocation.component.css',
})
export class PortfolioSecurityAllocationComponent implements OnInit {
  @Input() portfolioId!: number;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public type: ChartType = 'pie';
  public data: ChartData<'pie', number[], string> = {
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

    this.chartService.getSecurityAllocation(this.portfolioId).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = "No holdings found.";
          this.chart?.update();
          return;
        }

        this.data = {
          labels: res.map(x => x.securityCode),
          datasets: [
            {
              data: res.map(x => x.quantity),
              backgroundColor: [
                '#007bff', '#28a745', '#dc3545', '#ffc107', '#6f42c1',
                '#20c997', '#fd7e14', '#6610f2', '#e83e8c', '#17a2b8'
              ],
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
