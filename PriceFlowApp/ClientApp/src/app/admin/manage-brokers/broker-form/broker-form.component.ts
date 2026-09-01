import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Broker } from '../Broker';
import { TranslateModule } from '@ngx-translate/core';
import { BrokersService } from '../../../brokers/brokers.service';

@Component({
  selector: 'app-broker-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './broker-form.component.html',
  styleUrl: './broker-form.component.css',
})
export class BrokerFormComponent implements OnInit, OnChanges {
  @Input() brokerToEdit!: Broker;
  @Output() close = new EventEmitter<Broker | null>();

  form!: FormGroup;
  errorMessage: string | null= null;
  successMessage: string | null = null;

  companyError: string | null = null;
  commissionError: string | null = null;

  constructor(private fb: FormBuilder, private brokersService: BrokersService) { }

  ngOnInit(): void { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['brokerToEdit'] && this.brokerToEdit) {
      this.form = this.fb.group({
        company: [this.brokerToEdit.company || '', Validators.required],
          commissionPercent: [this.brokerToEdit.commissionPercent ?? 0, [Validators.required, Validators.min(0)]]
      });

      this.form.get('company')?.valueChanges.subscribe(() => {
        this.validateCompany();
      });

      this.form.get('commissionPercent')?.valueChanges.subscribe(() => {
        this.validateCommission();
      });
    }
  }

  validateCompany(): void {
    this.errorMessage = null;

    const company = this.form.get('company')?.value?.trim() || '';
    if (company === '') {
      this.companyError = 'ERRORS.COMPANY_VALIDATION_REQUIRED';
    } else {
      this.companyError = null;
    }
  }

  validateCommission(): void {
    this.errorMessage = null;

    const commission = this.form.get('commissionPercent')?.value;
    this.commissionError = null;

    if (commission === null || commission === undefined || commission === '') {
      this.commissionError = 'ERRORS.COMMISSION_VALIDATION_REQUIRED';

      return;
    }

    if (Number(commission) < 0) {
      this.commissionError = 'ERRORS.COMMISSION_INVALID';

      return;
    }
  }

  isFormValid(): boolean {
    const company = this.form.get('company')?.value?.trim() || '';
    const commission = this.form.get('commissionPercent')?.value;

    return company !== '' &&
      commission !== null &&
      commission !== undefined &&
      commission !== '' &&
      Number(commission) >= 0 &&
      !this.companyError &&
      !this.commissionError;
  }

  saveBroker(): void {
    this.successMessage = null;
    this.errorMessage = null;

    this.validateCompany();
    this.validateCommission();

    if (!this.isFormValid()) {
      return;
    }

    const updatedBroker: Broker = { ...this.brokerToEdit, ...this.form.value };

    if (updatedBroker.id === 0) {
      this.brokersService.add(updatedBroker)
        .subscribe({
          next: (b) => {
            this.successMessage = 'BROKERS.ADD_SUCCESS';
            setTimeout(() => this.close.emit(b), 800);
          },
          error: (err) => {
            this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
              : 'BROKERS.ADD_ERROR';
          }
        });
    }
        else {
        this.brokersService.update(this.brokerToEdit.id, updatedBroker)
          .subscribe({
            next: () => {
              this.successMessage = 'BROKERS.UPDATE_SUCCESS';
              setTimeout(() => this.close.emit(updatedBroker), 800);
            },
            error: (err) => {
              this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
                : 'BROKERS.UPDATE_ERROR';
            }
          });
      }
  }

  cancel(): void {
    this.close.emit(null);
  }
}
