import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Portfolio, PortfoliosService } from '../portfolios.service';
import { ActivatedRoute } from '@angular/router';
import { TransactionsComponent } from '../../transactions/transactions.component';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-portfolio-details',
  standalone: true,
  imports: [CommonModule, TransactionsComponent],
  templateUrl: './portfolio-details.component.html',
  styleUrl: './portfolio-details.component.css',
})
export class PortfolioDetailsComponent implements OnInit {
  selectedPortfolio?: Portfolio;

  constructor(private route: ActivatedRoute, private portfoliosService: PortfoliosService) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.route.paramMap.pipe(
      switchMap(params =>
        this.portfoliosService.getById(Number(params.get('id')))
      )
    ).subscribe(p => this.selectedPortfolio = p);
  }
}
