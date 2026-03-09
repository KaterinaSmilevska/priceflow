import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Broker } from '../Broker';
import { AdminService } from '../../admin.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-edit-broker',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './edit-broker.component.html',
  styleUrl: './edit-broker.component.css',
})
export class EditBrokerComponent implements OnInit {
  @Input() brokerToEdit!: Broker;
  @Output() close = new EventEmitter<Broker | null>();

  editForm!: FormGroup;
  errorMessage: string | null= null;
  successMessage: string | null = null;

  constructor(private fb: FormBuilder, private adminService: AdminService) { }

  ngOnInit(): void { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['brokerToEdit'] && this.brokerToEdit) {
      this.editForm = this.fb.group({
        company: [this.brokerToEdit.company || '', Validators.required],
          commissionPercent: [this.brokerToEdit.commissionPercent || 0, [Validators.required, Validators.min(0)]]
      });
    }
  }

  saveBroker(): void {
    if (this.editForm.invalid) return;

    const updatedBroker: Broker = { ...this.brokerToEdit, ...this.editForm.value };

    if (updatedBroker.id === 0) {
      this.adminService.addBroker(updatedBroker)
        .subscribe({
          next: (b) => {
            this.successMessage = 'Broker added successfully!';
            setTimeout(() => this.close.emit(b), 800);
          },
          error: () => this.errorMessage = 'Failed to add broker.'
        });
    }
        else {
        this.adminService.updateBroker(updatedBroker)
          .subscribe({
            next: () => {
              this.successMessage = 'Broker updated successfully!';
              setTimeout(() => this.close.emit(updatedBroker), 800);
            },
            error: () => this.errorMessage = 'Failed to update broker.'
          });
      }
  }

  cancel(): void {
    this.close.emit(null);
  }
}
