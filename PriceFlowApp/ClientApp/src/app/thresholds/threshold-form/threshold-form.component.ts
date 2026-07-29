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

  editForm!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(private fb: FormBuilder, private thresholdService: ThresholdService) { }

  ngOnInit(): void {
    this.editForm = this.fb.group({
      hvId: [null, Validators.required],
      lowerThreshold: [null, Validators.required],
      upperThreshold: [null, Validators.required],
    });

    this.loadEditData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['thresholdToEdit'] || changes['owned'] || changes['existingThresholds']) {
      this.loadEditData();
    }
  }

  private loadEditData(): void {
    if(!this.editForm) {
      return;
    }

    if (this.thresholdToEdit) {
      this.editForm.patchValue({
        hvId: this.thresholdToEdit.hvId,
        lowerThreshold: this.thresholdToEdit.lowerThreshold,
        upperThreshold: this.thresholdToEdit.upperThreshold
      });

      this.editForm.get('hvId')?.disable();
    }
    else {
      this.editForm.reset();
      this.editForm.get('hvId')?.enable();
    }
  }

  saveThreshold(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.errorMessage = null;

    const lower = this.editForm.value.lowerThreshold;
    const upper = this.editForm.value.upperThreshold;

    if (lower >= upper) {
      this.errorMessage = 'THRESHOLD_VALIDATION_ERROR';
      return;
    }

    if (this.thresholdToEdit) {
      const updateRequest: UpdateThresholdRequest = {
        lowerThreshold: lower,
        upperThreshold: upper
      };

      this.thresholdService.updateThreshold(this.thresholdToEdit.id, updateRequest)
        .subscribe({
          next: () => {
            this.successMessage = 'THRESHOLD.UPDATE_SUCCESS';
            setTimeout(() => this.saved.emit(), 800);
          },
          error: () => this.errorMessage = 'THRESHOLD.UPDATE_ERROR'
        });
    }
    else {
      const createRequest: CreateThresholdRequest = {
        hvId: this.editForm.value.hvId,
        lowerThreshold: lower,
        upperThreshold: upper
      };

      this.thresholdService.addThreshold(createRequest)
        .subscribe({
          next: () => {
            this.successMessage = 'THRESHOLD.ADD_SUCCESS';
            setTimeout(() => this.saved.emit(), 800);
          },
          error: () => this.errorMessage = 'THRESHOLD.ADD_ERROR'
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
    const lower = this.editForm?.get('lowerThreshold')?.value;
    const upper = this.editForm?.get('upperThreshold')?.value;

    return lower !== null && upper !== null && lower >= upper;
  }
}
