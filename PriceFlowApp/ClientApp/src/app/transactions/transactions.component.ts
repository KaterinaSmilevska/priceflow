import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { Transaction, TransactionsService } from './transactions.service';
import { TransactionFormComponent } from './transaction-form/transaction-form.component';
import { PortfolioAnalytics } from '../portfolios/portfolios.service';
import { PortfolioSecurityAllocationComponent } from '../market-overview/portfolio-security-allocation/portfolio-security-allocation.component';
import { PortfolioIncomeComponent } from '../market-overview/portfolio-income/portfolio-income.component';
import { PortfolioReturnsComponent } from '../portfolios/portfolio-returns/portfolio-returns.component';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, TransactionFormComponent, PortfolioIncomeComponent, PortfolioSecurityAllocationComponent, PortfolioReturnsComponent],
  templateUrl: './transactions.component.html',
  styleUrl: './transactions.component.css',
})
export class TransactionsComponent implements OnInit {
  @Input() portfolioId!: number;

  transactions: Transaction[] = [];
  showDeleteModal = false;
  showEditModal = false;
  loading: boolean = false;
  errorMessage: string | null = null;

  transactionToEdit: Transaction | undefined = undefined;
  transactionToDelete: Transaction | null = null;

  analytics?: PortfolioAnalytics;

  displayLimit = 5;
  showAll = false;

  constructor(private transactionsService: TransactionsService) { }

  ngOnInit(): void {
    this.loadTransactions();
    this.loadAnalytics();
  }

  loadTransactions() {
    this.transactionsService.getByPortfolioId(this.portfolioId).subscribe({
      next: data => {
        this.transactions = data;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load transactions';
        this.loading = false;
      }
    });
  }

  loadAnalytics() {
    this.transactionsService
      .getAnalytics(this.portfolioId)
      .subscribe(a => this.analytics = a);
  }

  openDeleteModal(transaction: Transaction) {
    this.transactionToDelete = transaction;
    this.showDeleteModal = true;
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.transactionToDelete = null;
  }

  confirmDelete() {
    if (!this.transactionToDelete) return;

    this.transactionsService.delete(this.portfolioId, this.transactionToDelete.id).subscribe({
      next: () => {
        this.transactions = this.transactions.filter(t => t.id !== this.transactionToDelete?.id);
        this.closeDeleteModal();
      },
      error: (err) => {
        console.error('Failed to delete transaction:', err);
        this.errorMessage = 'Failed to delete transaction.';
        this.closeDeleteModal();
      }
    });
  }

  openAddModal() {
    this.transactionToEdit = undefined;
    this.showEditModal = true;
  }

  openEditModal(transaction: Transaction) {
    this.transactionToEdit = transaction;
    this.showEditModal = true;
  }

  onModalClose(updatedTransaction: Transaction | null) {
    this.showEditModal = false;

    if (!updatedTransaction) return;

    this.loadTransactions();
    this.loadAnalytics();
  }

  get visibleTransactions(): Transaction[] {
    if (this.showAll) {
      return this.transactions;
    }
    return this.transactions.slice(0, this.displayLimit);
  }

  toggleShowAll() {
    this.showAll = !this.showAll;
  }

  getTotalCommission(t: Transaction): number {
    const base = t.sharesQuantity * t.sharesUnitPrice;
    const commissionPercent = (t.stockExchangeCommission ?? 0) +
      (t.brokerageCommission ?? 0) +
      (t.cdhvCommission ?? 0)
    return (
      base * commissionPercent / 100
    );
  }

  getCashFlow(t: Transaction): number {
    const base = t.sharesQuantity * t.sharesUnitPrice;
    const commission = this.getTotalCommission(t);

    if (t.typeTransaction === 'Купување') {
      return -(base + commission);
    }
    return (base - commission);
  }
}
