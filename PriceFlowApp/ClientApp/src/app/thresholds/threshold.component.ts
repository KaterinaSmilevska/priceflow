import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ThresholdService } from './threshold.service';
import { Threshold } from './Threshold';
import { OwnedSecurity } from './OwnedSecurity';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TransactionsService } from '../transactions/transactions.service';

@Component({
  selector: 'app-threshold',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './threshold.component.html',
  styleUrl: './threshold.component.css',
})
export class ThresholdComponent implements OnInit {

  owned: OwnedSecurity[] = [];
  thresholds: Threshold[] = [];

  editingId: number | null = null;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, private thresholdService: ThresholdService) { }

  form = this.fb.group({
    hvId: [null as number | null, Validators.required,],
    lowerThreshold: [null as number | null, Validators.required],
    upperThreshold: [null as number | null, Validators.required]
  });

  ngOnInit(): void {
    this.loadOwned();
    this.loadThresholds();
  }

  loadOwned() {
    this.thresholdService.getOwned()
      .subscribe(data => this.owned = data);
  }

  loadThresholds() {
    this.thresholdService.getUserThresholds()
      .subscribe(data => this.thresholds = data);
  }

  submit() {
    this.errorMessage = null;

    if (this.form.invalid) return;

    const hvId = this.form.value.hvId!;
    const lower = this.form.value.lowerThreshold!;
    const upper = this.form.value.upperThreshold!;

    if (lower >= upper) {
      this.errorMessage = 'Lower threshold must be less than upper threshold.';
      return;
    }

    if (this.editingId) {
      const updatePayload = {
        lowerThreshold: lower,
        upperThreshold: upper
      };

      this.thresholdService.updateThreshold(this.editingId, updatePayload)
        .subscribe(() => {
          this.resetForm();
          this.loadThresholds();
        });
    }
    else {
      const createPayload = {
        hvId: hvId,
        lowerThreshold: lower,
        upperThreshold: upper
      };

      this.thresholdService.addThreshold(createPayload)
          .subscribe({
            next: () => {
              this.resetForm();
              this.loadThresholds();
            },
            error: err => {
              this.errorMessage = err.error?.message || 'Threshold already exists.';
            }
          });
        }
  }

    edit(th: Threshold) {
      this.editingId = th.id;

      this.form.patchValue({
        hvId: th.hvId,
        lowerThreshold: th.lowerThreshold,
        upperThreshold: th.upperThreshold
      });

      this.form.get('hvId')?.disable();
  }

  delete(id: number) {
    this.thresholdService.deleteThreshold(id)
      .subscribe(() => this.loadThresholds());
  }

    resetForm() {
      this.editingId = null;
      this.form.reset();

      this.form.get('hvId')?.enable();
  }

  isSecurityAlreadyUsed(hvId: number): boolean {
    return this.thresholds.some(t => t.hvId === hvId);
  }
}
