import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, SimpleChange, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartData, ChartType, registerables } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { ChartService } from '../chart/chart.service';
import ChartDataLabels from 'chartjs-plugin-datalabels';

Chart.register(...registerables, ChartDataLabels);

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

  public options = {
    responsive: true,
    maintainAspectRatio: false
  };

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
        this.noDataMessage = "An error occured while loading data.";
      }
    });
  }
}
