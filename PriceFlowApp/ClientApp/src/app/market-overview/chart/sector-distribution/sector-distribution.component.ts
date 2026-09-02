import { CommonModule } from "@angular/common";
import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild } from "@angular/core";
import { Chart, ChartData, ChartType, registerables } from "chart.js";
import { ChartService } from "../chart.service";
import { BaseChartDirective } from 'ng2-charts';
import { FormsModule } from "@angular/forms";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { SectorsTranslatePipe } from "../../../shared/sectors-translate.pipe";
import { Subscription } from "rxjs";

Chart.register(...registerables);

@Component({
  selector: 'app-sector-distribution',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, TranslateModule],
  templateUrl: './sector-distribution.component.html',
})

export class SectorDistributionComponent implements OnInit, OnChanges, OnDestroy {
  @Input() date!: string;
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  private langSubscription?: Subscription;

  public type: ChartType = 'pie';
  public data: ChartData<'pie', number[], string> = {
    labels: [],
    datasets: [],
  };

  public securities: { id: number, code: string }[] = [];
  public selectedSecurityId: number | null = null;
  public noDataMessage: string | null = null;
  public defaultDate = new Date().getDay() - 7;

  constructor(private chartService: ChartService, private translateService: TranslateService,
    private sectorsTranslatePipe: SectorsTranslatePipe) { }

  ngOnInit(): void {
    if (!this.date) {
      this.chartService.getLatestDate().subscribe({
        next: (latestDate) => {
          this.date = latestDate;
          this.loadChart();
        },
        error: () => {
          this.noDataMessage = 'NO_SECTOR_DATA_AVAILABLE';
        }
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
    if (changes['date'] && !changes['date'].firstChange) {
      this.loadChart();
    }
  }

  ngOnDestroy(): void {
    this.langSubscription?.unsubscribe();
  }

  loadChart(): void {
    this.noDataMessage = null;

    this.chartService.getSectorDistribution(this.date).subscribe({
      next: (res) => {
        if (!res || res.length === 0) {
          this.data = { labels: [], datasets: [] };
          this.noDataMessage = this.translateService.instant('NO_DATA_AVAILABLE_FOR', { date: this.date });
          this.chart?.update();
          return;
        }

        const values = res.map((x) => x.marketCap);
        const colors = this.chartService.generateColors(values.length);
        this.data = {
          labels: res.map((x) =>
            this.translateService.instant(this.sectorsTranslatePipe.transform(x.sectorName))
          ),
          datasets: [
            {
              data: values,
              backgroundColor: colors,
            },
          ],
        };
        this.chart?.update();
      },
      error: (err) => {
        console.error(err);
        this.noDataMessage = 'LOADING_DATA_ERROR';
      },
    });
  }

  onFilterChange(): void {
    this.loadChart();
  }
}
