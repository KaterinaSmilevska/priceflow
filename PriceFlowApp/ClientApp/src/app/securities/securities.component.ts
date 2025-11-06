import { Component, OnInit } from '@angular/core';
import { SecuritiesService, Security } from './securities.service';

@Component({
  selector: 'app-securities',
  templateUrl: './securities.component.html',
  styleUrls: ['./securities.component.css']
})
export class SecuritiesComponent implements OnInit {
  securities: Security[] = [];
  loading:boolean = true;
  errorMessage:string | null = null;

  constructor(private securitiesService: SecuritiesService) { }

  ngOnInit(): void {
    this.loadSecurities();
  }

  loadSecurities(): void {
    this.securitiesService.getAll().subscribe({
      next: (securities) => {
        this.securities = securities;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = 'Could not load securities.';
        this.loading = false;
      }
    });
  }
}
