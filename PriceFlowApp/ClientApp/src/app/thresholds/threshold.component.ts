import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ThresholdService } from './threshold.service';
import { Threshold } from './Threshold';
import { OwnedSecurity } from './OwnedSecurity';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ThresholdFormComponent } from './threshold-form/threshold-form.component';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-threshold',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, TranslateModule, ThresholdFormComponent],
  templateUrl: './threshold.component.html',
  styleUrl: './threshold.component.css',
})
export class ThresholdComponent implements OnInit {

  owned: OwnedSecurity[] = [];
  thresholds: Threshold[] = [];

  showEditModal = false;
  thresholdToEdit?: Threshold;
  showDeleteModal = false;
  thresholdToDelete?: Threshold;

  editingId: number | null = null;
  successMessage: string | null = null;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, private thresholdService: ThresholdService, private router: Router) { }

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

  openAddModal() {
    this.thresholdToEdit = undefined;
    this.showEditModal = true;
  }

  openEditModal(threshold: Threshold) {
    this.thresholdToEdit = threshold;
    this.showEditModal = true;
  }

  onEditModalClose() {
    this.showEditModal = false;
    this.thresholdToEdit = undefined;
  }

  onSaved() {
    this.showEditModal = false;
    this.thresholdToEdit = undefined;
    this.loadThresholds();
  }

  openDeleteModal(threshold: Threshold) {
    this.thresholdToDelete = threshold;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    if (!this.thresholdToDelete) return;

    this.thresholdService
      .delete(this.thresholdToDelete.id)
      .subscribe({
        next: () => {
          this.closeDeleteModal();
          this.successMessage = 'THRESHOLD.DELETE_SUCCESS';
          setTimeout(() => this.successMessage = null, 800);
        },
        error: (err) => {
          this.errorMessage = `ERRORS.${err.error.code}`;
          this.closeDeleteModal();
        }
      });
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.thresholdToDelete = undefined;
  }

    resetForm() {
      this.editingId = null;
      this.form.reset();

      this.form.get('hvId')?.enable();
  }
}
