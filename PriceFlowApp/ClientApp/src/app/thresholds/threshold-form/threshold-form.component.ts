import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter, SimpleChanges, OnInit, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { Threshold } from '../Threshold';
import { ThresholdService } from '../threshold.service';
import { OwnedSecurity } from '../OwnedSecurity';
import { UpdateThresholdRequest } from '../UpdateThresholdRequest';
import { CreateThresholdRequest } from '../CreateThresholdRequest';

@Component({
  selector: 'app-threshold-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './threshold-form.component.html',
  styleUrl: './threshold-form.component.css',
})
export class ThresholdFormComponent implements OnInit, OnChanges {
  @Input() thresholdToEdit?: Threshold;
  @Input() owned: OwnedSecurity[] = [];
  @Input() existingThresholds: Threshold[] = [];

  @Output() saved = new EventEmitter<Threshold | null>();
  @Output() close = new EventEmitter<Threshold | null>();

  form!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  lowerThresholdError: string | null = null;
  upperThresholdError: string | null = null;
  thresholdRangeError: string | null = null;

  constructor(private fb: FormBuilder, private thresholdService: ThresholdService) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      hvId: [null, Validators.required],
      lowerThreshold: [0, Validators.required],
      upperThreshold: [1, Validators.required],
    });

    this.form.get('lowerThreshold')?.valueChanges.subscribe(() => {
      this.validateLowerThreshold();
      this.validateThresholdRange();
    });

    this.form.get('upperThreshold')?.valueChanges.subscribe(() => {
      this.validateUpperThreshold();
      this.validateThresholdRange();
    });

    this.loadEditData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['thresholdToEdit'] || changes['owned'] || changes['existingThresholds']) {
      this.loadEditData();
    }
  }

  private loadEditData(): void {
    if(!this.form) {
      return;
    }

    this.errorMessage = null;
    this.successMessage = null;
    this.lowerThresholdError = null;
    this.upperThresholdError = null;
    this.thresholdRangeError = null;

    if (this.thresholdToEdit) {
      this.form.patchValue({
        hvId: this.thresholdToEdit.hvId,
        lowerThreshold: this.thresholdToEdit.lowerThreshold,
        upperThreshold: this.thresholdToEdit.upperThreshold
      });

      this.form.get('hvId')?.disable();
    }
    else {
      this.form.reset();
      this.form.get('hvId')?.enable();
    }
  }

  validateLowerThreshold(): void {
    this.errorMessage = null;

    const lower = this.form.get('lowerThreshold')?.value;
    if (lower === null || lower === undefined || lower === '') {
      this.lowerThresholdError = 'ERRORS.LOWER_THRESHOLD_REQUIRED';
    } else {
      this.lowerThresholdError = null;
    }
  }

  validateUpperThreshold(): void {
    this.errorMessage = null;

    const upper = this.form.get('upperThreshold')?.value;
    if (upper === null || upper === undefined || upper === '') {
      this.upperThresholdError = 'ERRORS.UPPER_THRESHOLD_REQUIRED';
    } else {
      this.upperThresholdError = null;
    }
  }

  validateThresholdRange(): void {
    this.errorMessage = null;

    const lower = this.form.get('lowerThreshold')?.value;
    const upper = this.form.get('upperThreshold')?.value;

    this.thresholdRangeError = null;

    if (lower === null || lower === undefined || lower === '' ||
      upper === null || upper === undefined || upper === '') {
      return;
    }

    if (Number(lower) >= Number(upper)) {
      this.thresholdRangeError = 'ERRORS.INVALID_THRESHOLD_RANGE';
    }
  }

  isFormValid(): boolean {
    const lower = this.form.get('lowerThreshold')?.value;
    const upper = this.form.get('upperThreshold')?.value;

    const validLower = lower !== null && lower !== undefined && lower !== '';
    const validUpper = upper !== null && upper !== undefined && upper !== '';

    const validRange = validLower && validUpper && Number(lower) < Number(upper);

    return validLower && validUpper && validRange &&
      !this.lowerThresholdError &&
      !this.upperThresholdError &&
      !this.thresholdRangeError;
  }

  saveThreshold(): void {
    this.successMessage = null;
    this.errorMessage = null;

    this.validateLowerThreshold();
    this.validateUpperThreshold();
    this.validateThresholdRange();

    if (!this.isFormValid()) {
      this.form.markAllAsTouched();
      return;
    }

    const lower = this.form.value.lowerThreshold;
    const upper = this.form.value.upperThreshold;

    if (lower >= upper) {
      this.errorMessage = `ERRORS.INVALID_THRESHOLD_RANGE`;
      return;
    }

    if (this.thresholdToEdit) {
      const updateRequest: UpdateThresholdRequest = {
        lowerThreshold: lower,
        upperThreshold: upper
      };

      this.thresholdService.update(this.thresholdToEdit.id, updateRequest)
        .subscribe({
          next: () => {
            this.successMessage = 'THRESHOLD.UPDATE_SUCCESS';
            setTimeout(() => this.saved.emit(), 800);
          },
          error: (err) => {
            this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
              : 'THRESHOLD.UPDATE_ERROR';
          }
        });
    }
    else {
      const createRequest: CreateThresholdRequest = {
        hvId: this.form.value.hvId,
        lowerThreshold: lower,
        upperThreshold: upper
      };

      this.thresholdService.add(createRequest)
        .subscribe({
          next: () => {
            this.successMessage = 'THRESHOLD.ADD_SUCCESS';
            setTimeout(() => this.saved.emit(), 800);
          },
          error: (err) => {
            this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
              : 'THRESHOLD.ADD_ERROR';
          }
        });
    }
  }

  onCancel(): void {
    this.close.emit(null);
  }

  isSecurityAlreadyUsed(hvId: number): boolean {
    return this.existingThresholds.some(t => t.hvId === hvId);
  }

  get invalidThresholdRange(): boolean {
    const lower = this.form?.get('lowerThreshold')?.value;
    const upper = this.form?.get('upperThreshold')?.value;

    return lower !== null && lower !== undefined && lower !== '' &&
      upper !== null && upper !== undefined && upper !== '' &&
    Number(lower) >= Number(upper);
  }
}
