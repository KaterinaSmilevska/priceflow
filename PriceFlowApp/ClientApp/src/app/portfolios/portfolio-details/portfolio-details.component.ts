import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Portfolio, PortfoliosService } from '../portfolios.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-portfolio-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './portfolio-details.component.html',
  styleUrl: './portfolio-details.component.css',
})
export class PortfolioDetailsComponent implements OnInit {
  portfolio!: Portfolio;

  constructor(private route: ActivatedRoute, private portfoliosService: PortfoliosService) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.portfoliosService.getAll().subscribe(res => {
      this.portfolio = res.find(p => p.id === id)!;
    });
  }
}
