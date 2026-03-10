import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { PriceTrendComponent } from './price-trend/price-trend.component';
import { SectorDistributionComponent } from './sector-distribution/sector-distribution.component';
import { FormsModule } from "@angular/forms";
import { ChartService } from "./chart.service";
import { TranslateModule } from "@ngx-translate/core";
import { Security } from "./Security";

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [CommonModule, FormsModule, PriceTrendComponent, SectorDistributionComponent, TranslateModule],
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css'],
})

export class ChartComponent implements OnInit {
  public securities: Security[] = [];
  public selectedSecurityId!: number;

  startDate!: string;
  endDate!: string;
  sectorDate!: string;

  constructor(public chartService: ChartService) {
    const now = new Date();
    this.endDate = this.chartService.formatDate(now);
    this.startDate = this.chartService.formatDate(new Date(now.setMonth(now.getMonth() - 3)));
    this.sectorDate = this.chartService.formatDate(new Date());
  }

  ngOnInit(): void {
    this.chartService.getSecurities().subscribe({
      next: (securities) => {
        this.securities = securities;
        if (securities.length)
          this.selectedSecurityId = securities[0].id;
      },
      error: err => console.error(err),
    });
  }

  onFilterChange(): void {
    this.startDate = this.startDate;
    this.endDate = this.endDate;
    this.sectorDate = this.sectorDate;
  }
}
