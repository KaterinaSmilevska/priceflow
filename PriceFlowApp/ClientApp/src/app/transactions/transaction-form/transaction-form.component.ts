import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TransactionsService } from '../transactions.service';
import { SecuritiesService } from '../../securities/securities.service';
import { Tooltip } from 'bootstrap';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
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
export class TransactionFormComponent implements OnInit, OnChanges {
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
  priceWarning: string | null = null;

  successMessage: string | null = null;
  errorMessage: string | null = null;

  sharesQuantityError: string | null = null;
  sharesUnitPriceError: string | null = null;

  constructor(private fb: FormBuilder,
    private transactionsService: TransactionsService,
    private securitiesService: SecuritiesService,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      hvCode: [null, Validators.required],
      sharesQuantity: [1, [Validators.required, Validators.min(1)]],
      sharesUnitPrice: [0, [Validators.required, Validators.min(0.01)]],
      typeTransaction: [null, Validators.required],
      isReal: [true],
      stockExchangeCommission: [{ value: 0.2, disabled: true }],
      brokerageCommission: [{ value: 0.75, disabled: true }],
      cdhvCommission: [{ value: 0.1, disabled: true }],
      date: [new Date().toISOString().substring(0, 10), Validators.required],
      amount: [{ value: 0, disabled: true }]
    });

    this.securitiesService.getAll().subscribe(s => {
      this.securities = s;
    });

    if (this.transaction) {
      this.loadTransactionData();
    }

    this.form.get('hvCode')?.valueChanges.subscribe(() => {
      this.updateShares();
      this.loadPrices();
    });

    this.form.get('isReal')?.valueChanges.subscribe(() => {
      this.updateShares();
      this.loadPrices();
    });

    this.form.get('date')?.valueChanges
      .subscribe(() => this.loadPrices());

    this.form.valueChanges.subscribe(() => {
      this.updateShares();
      this.loadPrices();
      this.calculateAmountPreview();
      this.evaluateLimits();
      this.validateSellDate();
    });

    this.form.get('sharesQuantity')?.valueChanges.subscribe(() => {
      this.validateSharesQuantity();
      this.evaluateLimits();
      this.validateSellDate();
      this.calculateAmountPreview();
    })

    this.form.get('sharesUnitPrice')?.valueChanges.subscribe(() => {
      this.validateSharesUnitPrice();
      this.validatePriceRange();
      this.calculateAmountPreview();
    })

    setTimeout(() => {
      document.querySelectorAll('[data-bs-toggle="tooltip"]')
        .forEach(el => new Tooltip(el));
    });
  }

  validateSharesQuantity(): void {
    this.errorMessage = null;

    const quantity = this.form.get('sharesQuantity')?.value;
    if (quantity === null || quantity === undefined || quantity === '') {
      this.sharesQuantityError = 'ERRORS.SHARES_QUANTITY_VALIDATION_REQUIRED';
      return;
    }

    if (Number(quantity) < 1) {
      this.sharesQuantityError = 'ERRORS.INVALID_SHARES_QUANTITY';
      return;
    }

    this.sharesQuantityError = null;
  }

  validateSharesUnitPrice(): void {
    this.errorMessage = null;

    const unitPrice = this.form.get('sharesUnitPrice')?.value;
    if (unitPrice === null || unitPrice === undefined || unitPrice === '') {
      this.sharesUnitPriceError = 'ERRORS.SHARES_UNITPRICE_VALIDATION_REQUIRED';
      return;
    }

    if (Number(unitPrice) < 0.01) {
      this.sharesUnitPriceError = 'ERRORS.INVALID_SHARES_UNITPRICE';
      return;
    }

    this.sharesUnitPriceError = null;
  }

  isFormValid(): boolean {
    const quantity = this.form.get('sharesQuantity')?.value;
    const unitPrice = this.form.get('sharesUnitPrice')?.value;

    return quantity !== null &&
      quantity !== undefined &&
      quantity !== '' &&
      unitPrice !== null &&
      unitPrice !== undefined &&
      unitPrice !== '' &&
      !this.sharesQuantityError &&
      !this.sharesUnitPriceError &&
      !this.sellLimitExceeded &&
      !this.buyLimitExceeded &&
      !this.dateSellInvalid;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['transaction'] && this.form) {
      this.loadTransactionData();
    }
  }

  loadTransactionData(): void {
    if (!this.transaction || !this.form) return;
    this.form.patchValue({
      hvCode: this.transaction.securityCode,
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

    this.transactionsService.getOwnedSharesAtDate(this.portfolioId, raw.hvCode, raw.isReal, raw.date, this.transaction?.id)
      .subscribe(owned => {
        this.ownedSharesAtDate = owned;
        this.dateSellInvalid = raw.sharesQuantity > owned;
      });
  }

  private loadPrices(): void {
    const code = this.form.get('hvCode')?.value;
    const date = this.form.get('date')?.value;

    if (!code) return;

    this.securitiesService.getLatestPrices(code, date).subscribe(prices => {
      this.dailyPrices = prices;

      this.validatePriceRange();
    })
  }

  private validatePriceRange(): void {
    this.priceWarning = null;

    if (!this.dailyPrices) return;

    const price = Number(this.form.get('sharesUnitPrice')?.value);

    if (!price) return;

    if (this.dailyPrices.minPrice != null && price < this.dailyPrices.minPrice) {
      this.priceWarning = this.translateService.instant('TRANSACTIONS.PRICE_BELOW_MIN_WARNING', { price: this.formatPrice(this.dailyPrices.minPrice) });
    }

    if (this.dailyPrices.maxPrice != null && price > this.dailyPrices.maxPrice) {
      this.priceWarning = this.translateService.instant('TRANSACTIONS.PRICE_ABOVE_MAX_WARNING', { price: this.formatPrice(this.dailyPrices.maxPrice) });
    }
  }

  private formatPrice(price: number): string {
    return new Intl.NumberFormat('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(price);
  }

  onSubmit(): void {
    this.errorMessage = null;
    this.successMessage = null;

    this.validateSharesQuantity();
    this.validateSharesUnitPrice();

    if (!this.isFormValid()) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.sellLimitExceeded || this.buyLimitExceeded || this.dateSellInvalid) return;

    const raw = this.form.getRawValue();

    const payload = {
      id: this.transaction?.id,
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

    this.successMessage = null;
    this.errorMessage = null;

    if (this.transaction) {
      this.transactionsService
        .update(this.portfolioId, this.transaction.id, payload)
        .subscribe({
          next: (updated) => {
            if (!updated) return;

            this.successMessage = 'TRANSACTIONS.UPDATE_SUCCESS';
            this.form.patchValue({ amount: updated.amount }, { emitEvent: false });

            setTimeout(() => { this.close.emit(updated); }, 800);
          },
          error: (err) => {
            this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
              : 'TRANSACTIONS.UPDATE_ERROR';
          }
        });
    } else {
      this.transactionsService
        .add(this.portfolioId, payload)
        .subscribe({
          next: (newTransaction) => {
            this.successMessage = 'TRANSACTIONS.ADD_SUCCESS';

            this.form.patchValue({ amount: newTransaction.amount }, { emitEvent: false });

            setTimeout(() => { this.close.emit(newTransaction); }, 800);
          },
          error: (err) => {
            this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
              : 'TRANSACTIONS.ADD_ERROR';
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
