import { Component, OnInit } from '@angular/core';
import { PortfoliosService, Portfolio, CreatePortfolio } from './portfolios.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AddPortfolioComponent } from './add-portfolio/add-portfolio.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-portfolios',
  standalone: true,
  imports: [CommonModule, FormsModule, AddPortfolioComponent, TranslateModule],
  templateUrl: './portfolios.component.html',
  styleUrl: './portfolios.component.css',
})
export class PortfoliosComponent implements OnInit {
  portfolios: Portfolio[] = [];
  showEditModal = false;
  showDeleteModal = false;
  errorMessage: string | null = null;

  portfolioToEdit?: Portfolio;
  portfolioToDelete: Portfolio | null = null;

  constructor(private portfoliosService: PortfoliosService, private router: Router) { }


  ngOnInit(): void {
    this.loadPortfolios();
  }

  loadPortfolios() {
    this.portfoliosService.getAll().subscribe({
      next: res => this.portfolios = res,
      error: () => this.errorMessage = "Error loading portfolios"
    });
  }

  view(id: number): void {
    this.router.navigate(['/portfolios', id]);
  }

  openAddModal() {
    this.portfolioToEdit = undefined;
    this.showEditModal = true;
  }

  openEditModal(portfolio: Portfolio) {
    this.portfolioToEdit = portfolio;
    this.showEditModal = true;
  }

  onModalClose(updated: Portfolio | null) {
    this.showEditModal = false;
    if (!updated) return;

    const index = this.portfolios.findIndex(p => p.id === updated.id);
    if (index > -1) this.portfolios[index] = updated;
    else this.portfolios.push(updated);
  }

  openDeleteModal(portfolio: Portfolio) {
    this.portfolioToDelete = portfolio;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.portfolioToDelete = null;
  }

  confirmDelete() {
    if (!this.portfolioToDelete) return;

    this.portfoliosService.deletePortfolio(this.portfolioToDelete.id).subscribe(() => {
      this.portfolios = this.portfolios.filter(p => p.id !== this.portfolioToDelete?.id);
      this.closeDeleteModal();
    });
  }
}
