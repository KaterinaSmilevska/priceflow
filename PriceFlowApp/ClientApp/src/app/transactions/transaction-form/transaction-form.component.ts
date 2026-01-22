import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Transaction, TransactionsService } from '../transactions.service';
import { SecuritiesService, Security } from '../../securities/securities.service';
import { Tooltip } from 'bootstrap';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './transaction-form.component.html',
  styleUrl: './transaction-form.component.css',
})
export class TransactionFormComponent implements OnInit {
  @Input() portfolioId!: number;
  @Input() transaction?: Transaction;

  @Output() close = new EventEmitter<Transaction | null>();

  form!: FormGroup;
  securities: Security[] = [];

  constructor(private fb: FormBuilder,
    private transactionsService: TransactionsService,
    private securitiesService: SecuritiesService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      hvCode: ['', Validators.required],
      sharesQuantity: [1, [Validators.required, Validators.min(1)]],
      sharesUnitPrice: [0, [Validators.required, Validators.min(0)]],
      typeTransaction: ['Купување', Validators.required],
      isReal: [true],
      stockExchangeCommission: [{ value: 0.2, disabled: true }],
      brokerageCommission: [{ value: 0.75, disabled: true }],
      cdhvCommission: [{ value: 0.1, disabled: true }],
      date: [new Date().toISOString().substring(0, 10), Validators.required],
      amount: [{ value: 0, disabled: true }]
    });

    this.securitiesService.getAll().subscribe(s => { this.securities = s; })

    if (this.transaction) {
      this.form.patchValue({
        hvCode: this.transaction.hvCode,
        sharesQuantity: this.transaction.sharesQuantity,
        sharesUnitPrice: this.transaction.sharesUnitPrice,
        typeTransaction: this.transaction.typeTransaction,
        isReal: this.transaction.isReal,
        stockExchangeCommission: this.transaction.stockExchangeCommission,
        brokerageCommission: this.transaction.brokerageCommission,
        cdhvCommission: this.transaction.cdhvCommission,
        date: this.transaction.date,
        amount: this.transaction.amount
      }, { emitEvent: false });

      this.calculateAmountPreview();
    }
    this.form.valueChanges.subscribe(() => this.calculateAmountPreview());

    setTimeout(() => {
      document.querySelectorAll('[data-bs-toggle="tooltip"]')
        .forEach(el => new Tooltip(el));
    });
  }

  private calculateAmountPreview(): void {
    const base = this.baseAmount;

    if (!isFinite(base)) {
      this.form.patchValue({ amount: 0 }, { emitEvent: false });
      return;
    }

    const total = this.totalCommission;

    const preview = this.form.getRawValue().typeTransaction === 'Купување'
      ? base + total
      : base - total;

    this.form.patchValue({ amount: Number(preview.toFixed(2)) }, { emitEvent: false });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    const raw = this.form.getRawValue();

    const payload = {
      hvCode: raw.hvCode,
      sharesQuantity: raw.sharesQuantity,
      sharesUnitPrice: raw.sharesUnitPrice,
      typeTransaction: raw.typeTransaction,
      isReal: raw.isReal,
      stockExchangeCommission: raw.stockExchangeCommission,
      brokerageCommission: raw.brokerageCommission,
      cdhvCommission: raw.cdhvCommission,
      date: raw.date
    };

    if (this.transaction) {
      this.transactionsService
        .update(this.portfolioId, this.transaction.id, payload)
        .subscribe((updated) => {
          if (!updated) return;

          this.form.patchValue({ amount: updated.amount }, { emitEvent: false });
          this.close.emit(updated);
        });
        
    } else {
      this.transactionsService
        .add(this.portfolioId, payload)
        .subscribe((newTransaction) => {
          this.form.patchValue({ amount: newTransaction.amount }, { emitEvent: false });
          this.close.emit(newTransaction);
        });
    }
  }

  onCancel(): void {
    this.close.emit(null);
  }

  get baseAmount(): number {
    const raw = this.form.getRawValue();
    const quantity = Number(raw.sharesQuantity) || 0;
    const unitPrice = Number(raw.sharesUnitPrice) || 0;
    return quantity * unitPrice;
  }

  get stockExchangeFee(): number {
    return this.baseAmount * Number(this.form.getRawValue().stockExchangeCommission) / 100;
  }

  get brokerageFee(): number {
    return this.baseAmount * Number(this.form.getRawValue().brokerageCommission) / 100;
  }

  get cdhvFee(): number {
    return this.baseAmount * Number(this.form.getRawValue().cdhvCommission) / 100;
  }

  get totalCommission(): number {
    return this.stockExchangeFee + this.brokerageFee + this.cdhvFee;
  }
}
