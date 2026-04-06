import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TransactionsService } from '../transactions.service';
import { SecuritiesService } from '../../securities/securities.service';
import { Tooltip } from 'bootstrap';
import { TranslateModule } from '@ngx-translate/core';
import { Transaction } from '../Transaction';
import { Security } from '../../securities/Security';
import { SecurityDailyPrices } from '../../securities/SecurityDailyPrices';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './transaction-form.component.html',
  styleUrl: './transaction-form.component.css',
})
export class TransactionFormComponent implements OnInit {
  @Input() portfolioId!: number;
  @Input() transaction?: Transaction;

  @Output() close = new EventEmitter<Transaction | null>();

  form!: FormGroup;
  securities: Security[] = [];

  dateSellInvalid = false;
  ownedSharesAtDate: number | null = null;

  ownedShares: number | null = null;
  totalShares: number | null = null;
  remainingShares: number | null = null;

  sellLimitExceeded = false;
  buyLimitExceeded = false;

  isReal = true;

  dailyPrices?: SecurityDailyPrices;

  constructor(private fb: FormBuilder,
    private transactionsService: TransactionsService,
    private securitiesService: SecuritiesService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      hvCode: ['', Validators.required],
      sharesQuantity: [1, [Validators.required, Validators.min(1)]],
      sharesUnitPrice: [{ value: 0, disabled: true }],
      selectedPriceType: ['average', Validators.required],
      typeTransaction: ['Купување', Validators.required],
      isReal: [true],
      stockExchangeCommission: [{ value: 0.2, disabled: true }],
      brokerageCommission: [{ value: 0.75, disabled: true }],
      cdhvCommission: [{ value: 0.1, disabled: true }],
      date: [new Date().toISOString().substring(0, 10), Validators.required],
      amount: [{ value: 0, disabled: true }]
    });

    this.securitiesService.getAll().subscribe(s => { this.securities = s; })

    this.form.get('hvCode')?.valueChanges.subscribe(() => {
      this.updateShares();
      this.loadPrices();
    });

    this.form.get('isReal')?.valueChanges.subscribe(() => {
      this.updateShares();
      this.loadPrices();
    });

    this.form.get('selectedPriceType')?.valueChanges
      .subscribe(() => this.applySelectedPrice());

    this.form.get('date')?.valueChanges
      .subscribe(() => this.loadPrices());

    this.form.valueChanges.subscribe(() => {
      this.updateShares();
      this.loadPrices();
      this.calculateAmountPreview();
      this.evaluateLimits();
      this.validateSellDate();
    })

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
      this.updateShares();
      this.loadPrices();
    }

    setTimeout(() => {
      document.querySelectorAll('[data-bs-toggle="tooltip"]')
        .forEach(el => new Tooltip(el));
    });
  }

  private updateShares(): void {
    const code = this.form.get('hvCode')?.value;

    if (!code) return;

    const isRealValue = this.form.get('isReal')?.value ?? true;

    this.transactionsService
      .getOwnedShares(this.portfolioId, code, isRealValue)
      .subscribe(val => {
        this.ownedShares = val;
        this.calculateRemainingShares();
      });

    this.transactionsService
      .getTotalShares(code)
      .subscribe(val => {
        this.totalShares = val;
        this.calculateRemainingShares();
      });
  }

  private calculateRemainingShares(): void {
    if (this.totalShares !== null && this.ownedShares !== null) {
      this.remainingShares = this.totalShares - this.ownedShares;
    } else {
      this.remainingShares = null;
    }
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

  private evaluateLimits(): void {
    const raw = this.form.getRawValue();

    this.sellLimitExceeded = false;
    this.buyLimitExceeded = false;

    if (!raw.sharesQuantity || !raw.typeTransaction) return;

    if (raw.typeTransaction === 'Продавање' && this.ownedShares !== null) {
      this.sellLimitExceeded = raw.sharesQuantity > this.ownedShares;
    }

    if (raw.typeTransaction === 'Купување' && this.remainingShares !== null) {
      this.buyLimitExceeded = raw.sharesQuantity > this.remainingShares;
    }
  }

  private validateSellDate(): void {
    const raw = this.form.getRawValue();

    if (raw.typeTransaction !== 'Продавање' || !raw.hvCode) {
      this.dateSellInvalid = false;
      return;
    }

    this.transactionsService.getOwnedSharesAtDate(this.portfolioId, raw.hvCode, raw.isReal, raw.date)
      .subscribe(owned => {
        this.ownedSharesAtDate = owned;
        this.dateSellInvalid = raw.sharesQuantity > owned;
      });
  }

  private inferPriceType() {
    if (!this.dailyPrices || !this.transaction) return;

    const unitPrice = this.transaction.sharesUnitPrice;

    const min = this.dailyPrices.minPrice;
    const average = this.dailyPrices.averagePrice;
    const max = this.dailyPrices.maxPrice;

    const isClose = (a: number, b: number) => Math.abs(a - b) < 0.0001;

    let type: 'min' | 'average' | 'max' = 'average';

    if (min != null && isClose(unitPrice, min)) {
      type = 'min';
    } else if (max != null && isClose(unitPrice, max)) {
      type = 'max';
    } else if (average != null && isClose(unitPrice, average)) {
      type = 'average';
    }
    this.form.patchValue({ selectedPriceType: type }, { emitEvent: false });
  }

  private loadPrices(): void {
    const code = this.form.get('hvCode')?.value;
    const date = this.form.get('date')?.value;

    if (!code) return;

    this.securitiesService.getLatestPrices(code, date).subscribe(prices => {
      this.dailyPrices = prices;
      if (this.transaction) {
        this.inferPriceType();
      } else {
        this.applySelectedPrice();
      }
      
    })
  }

  private applySelectedPrice(): void {
    if (!this.dailyPrices) return;

    const type = this.form.get('selectedPriceType')?.value;
    let price: number | null = null;

    switch (type) {
      case 'min':
        price = this.dailyPrices.minPrice;
        break;
      case 'max':
        price = this.dailyPrices.maxPrice;
        break;
      default:
        price = this.dailyPrices.averagePrice;
    }

    if (price == null) return;

    this.form.patchValue(
      { sharesUnitPrice: price },
      { emitEvent: false }
    );
    this.calculateAmountPreview();
  }

  onSubmit(): void {
    if (this.form.invalid || this.sellLimitExceeded || this.buyLimitExceeded || this.dateSellInvalid) return;

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
        .subscribe({
          next: (newTransaction) => {
            this.form.patchValue({ amount: newTransaction.amount }, { emitEvent: false });
            this.close.emit(newTransaction);
          },
          error: (err) => {
            if (err.status === 400 && err.error?.message) {
              this.form.setErrors({
                backend: err.error.message
              });
            }
          }
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
