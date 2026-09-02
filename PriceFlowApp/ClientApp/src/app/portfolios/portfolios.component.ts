import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { PortfoliosService } from './portfolios.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { PortfolioFormComponent } from './portfolio-form/portfolio-form.component';
import { TranslateModule } from '@ngx-translate/core';
import { Portfolio } from './Portfolio';

@Component({
  selector: 'app-portfolios',
  standalone: true,
  imports: [CommonModule, FormsModule, PortfolioFormComponent, TranslateModule, RouterModule],
  templateUrl: './portfolios.component.html',
  styleUrl: './portfolios.component.css',
})
export class PortfoliosComponent implements OnInit {
  portfolios: Portfolio[] = [];
  showEditModal = false;
  showDeleteModal = false;

  successMessage: string | null = null;
  errorMessage: string | null = null;

  portfolioToEdit?: Portfolio;
  portfolioToDelete: Portfolio | null = null;

  @Output() close = new EventEmitter<Portfolio | null>();

  constructor(private portfoliosService: PortfoliosService, private router: Router) { }

  ngOnInit(): void {
    this.loadPortfolios();
  }

  loadPortfolios() {
    this.portfoliosService.getAll().subscribe({
      next: res => this.portfolios = res,
      error: () => this.errorMessage = "LOADING_DATA_ERROR"
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

    this.portfoliosService.delete(this.portfolioToDelete.id)
      .subscribe({
        next: () => {
          this.portfolios = this.portfolios.filter(p => p.id !== this.portfolioToDelete?.id);
          this.closeDeleteModal();
          this.successMessage = 'PORTFOLIOS.DELETE_SUCCESS';
          setTimeout(() => this.successMessage = null, 800);
        },
        error: (err) => {
          this.errorMessage = `ERRORS.${err.error.code}`;
          this.closeDeleteModal();
        }
    });
  }
}
