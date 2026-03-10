import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { PortfolioTableView, PortfolioValue, Transaction, TransactionsService } from './transactions.service';
import { PortfolioAnalytics } from '../portfolios/portfolios.service';
import { PortfolioSecurityAllocationComponent } from '../market-overview/portfolio-security-allocation/portfolio-security-allocation.component';
import { PortfolioIncomeComponent } from '../market-overview/portfolio-income/portfolio-income.component';
import { PortfolioReturnsComponent } from '../portfolios/portfolio-returns/portfolio-returns.component';
import { PortfolioReturns, PortfolioReturnsService } from '../portfolios/portfolio-returns/portfolio-returns.service';
import { SecuritiesService } from '../securities/securities.service';
import { TransactionFormComponent } from './transaction-form/transaction-form.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-transactions',
  standalone: true,
  imports: [CommonModule, TransactionFormComponent, PortfolioIncomeComponent,
    PortfolioSecurityAllocationComponent, PortfolioReturnsComponent, TranslateModule],
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

  displayLimit = 10;
  showAll = false;
  filteredTableView: PortfolioTableView[] = [];

  securityCodeMap = new Map<number, string>();

  portfolioReturns: PortfolioReturns[] = [];
  tableView: PortfolioTableView[] = [];

  portfolioValue: PortfolioValue[] = [];
  loadingValue = false;
  isReal = true;
  
  constructor(private transactionsService: TransactionsService,
    private portfolioReturnsService: PortfolioReturnsService,
    private securitiesService: SecuritiesService) { }

  ngOnInit(): void {
    this.loadSecurities();
    this.loadTransactions();
    this.loadPortfolioReturns();
    this.loadAnalytics();
  }

  loadTransactions() {
    this.transactionsService.getByPortfolioId(this.portfolioId).subscribe({
      next: data => {
        this.transactions = data;
        this.loading = false;
        this.buildTableView();
      },
      error: () => {
        this.errorMessage = 'LOADING_DATA_ERROR';
        this.loading = false;
      }
    });
  }

  loadAnalytics() {
    this.transactionsService
      .getAnalytics(this.portfolioId, this.isReal)
      .subscribe(a => this.analytics = a);
  }

  loadPortfolioReturns() {
    this.portfolioReturnsService
      .getByPortfolioId(this.portfolioId)
      .subscribe(r => {
        this.portfolioReturns = r;
        this.buildTableView();
      })
  }

  loadSecurities() {
    this.securitiesService.getAll().subscribe(securities => {
      securities.forEach(s =>
        this.securityCodeMap.set(s.id, s.code)
      );
      this.buildTableView();
    });
  }

  buildTableView() {
    if (!this.transactions.length && this.portfolioReturns.length) return;

    const transactionRows: PortfolioTableView[] = this.transactions.map(t => ({
      date: t.date,
      hvCode: t.hvCode,
      type: t.typeTransaction,

      sharesQuantity: t.sharesQuantity,
      sharesUnitPrice: t.sharesUnitPrice,

      amount: t.amount,
      commission: this.getTotalCommission(t),
      cashFlow: this.getCashFlow(t),

      isReal: t.isReal,
      transactionId: t.id
    }));

    const dividendRows: PortfolioTableView[] = this.portfolioReturns.map(r => ({
      date: r.date,
      hvCode: this.securityCodeMap.get(r.hvId) ?? '-',
      type: 'Дивиденден принос',

      amount: r.netAmount,
      cashFlow: r.netAmount,

      isReal: true
    }));

    this.tableView = [...transactionRows, ...dividendRows]
      .sort((a, b) => +new Date(b.date) - +new Date(a.date))
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
        this.reloadAll();

        this.closeDeleteModal();
      },
      error: (err) => {
        this.errorMessage = 'TRANSACTIONS.DELETE_ERROR';
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

  get visibleTableView(): PortfolioTableView[] {
    this.filteredTableView = this.tableView
      .filter(t => t.isReal === this.isReal);

    if (this.showAll) {
      return this.filteredTableView;
    }
    return this.filteredTableView.slice(0, this.displayLimit);
  }

  toggleShowAll() {
    this.showAll = !this.showAll;
  }

  editFromTable(transactionId: number) {
    const transaction = this.transactions.find(t => t.id == transactionId);
    if (!transaction) return;

    this.openEditModal(transaction);
  }

  deleteFromTable(transactionId: number) {
    const transaction = this.transactions.find(t => t.id == transactionId);
    if (!transaction) return;

    this.openDeleteModal(transaction);
  }

  getTotalCommission(t: Transaction): number {
    const base = t.amount;
    const commissionPercent = (t.stockExchangeCommission ?? 0) +
      (t.brokerageCommission ?? 0) +
      (t.cdhvCommission ?? 0)
    return (
      base * commissionPercent / 100
    );
  }

  getCashFlow(t: Transaction): number {
    const commission = this.getTotalCommission(t);

    if (t.typeTransaction === 'Купување') {
      return -(t.amount + commission);
    }
    return (t.amount - commission);
  }

  setMode(value: boolean) {
    this.isReal = value;
    this.reloadAll();
  }

  reloadAll() {
    this.loadTransactions();
    this.loadPortfolioReturns();
    this.loadAnalytics();
    this.loadPortfolioValue();
  }

  loadPortfolioValue() {
    this.loadingValue = true;

    this.transactionsService
      .getPortfolioValue(this.portfolioId, this.isReal)
      .subscribe({
        next: res => {
          this.portfolioValue = res.filter(v => v.isReal === this.isReal);
          this.loadingValue = false;
        },
        error: () => this.loadingValue = false
      });
  }
}
