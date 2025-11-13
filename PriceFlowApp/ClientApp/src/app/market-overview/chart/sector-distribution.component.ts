import { CommonModule } from "@angular/common";
import { Component, Input, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { Chart, ChartData, ChartType, registerables } from "chart.js";
import { ChartService } from "./chart.service";
import { BaseChartDirective } from 'ng2-charts';
import { FormsModule } from "@angular/forms";

Chart.register(...registerables);

@Component({
  selector: 'app-sector-distribution',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './sector-distribution.component.html',
})

export class SectorDistributionComponent implements OnInit {
  @Input() date!: string;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  public type: ChartType = 'pie';
  public data: ChartData<'pie', number[], string> = {
    labels: [],
    datasets: [],
  };

  public securities: { id: number, code: string }[] = [];
  public selectedSecurityId: number | null = null;
  public noDataMessage: string | null = null;
  public defaultDate = new Date().getDay() - 7;

  constructor(private chartService: ChartService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['date'] && !changes['date'].firstChange) {
      this.loadChart();
    }
  }

  ngOnInit(): void {
    if (!this.date) {
      this.chartService.getLatestDate().subscribe({
        next: (latestDate) => {
          this.date = latestDate;
          this.loadChart();
        },
        error: () => {
          this.noDataMessage = "No sector data available.";
        }
      });
    }
    else {
      this.loadChart();
    }
    //this.chartService.getSecurities().subscribe(security => {
    //  this.securities = security.map(s => ({ id: s.id, code: s.code }));
    //  if (this.securities.length > 0) {
    //    this.date = this.chartService.formatDate(this.date);
    //    this.loadChart();
    //  }
    //});
  }

  loadChart(): void {
    this.noDataMessage = null;

    this.chartService.getSectorDistribution(this.date).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = `No data available for ${this.date}.`;
          this.chart?.update();
          return;
        }
        this.data = {
          labels: res.map((x) => x.sectorName),
          datasets: [
            {
              data: res.map((x) => x.marketCap),
              backgroundColor: [
                '#007bff', '#28a745', '#dc3545', '#ffc107', '#6f42c1',
                '#20c997', '#fd7e14', '#6610f2', '#e83e8c', '#17a2b8'
              ],
            },
          ],
        };
        this.chart?.update();
      },
      error: (err) => {
        console.error(err);
        this.noDataMessage = "An error occured while loading data.";
      },
    });
  }

  onFilterChange(): void {
    this.loadChart();
  }
}
