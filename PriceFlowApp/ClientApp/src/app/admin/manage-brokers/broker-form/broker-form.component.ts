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

  constructor(private fb: FormBuilder, private brokersService: BrokersService) { }

  ngOnInit(): void { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['brokerToEdit'] && this.brokerToEdit) {
      this.form = this.fb.group({
        company: [this.brokerToEdit.company || '', Validators.required],
          commissionPercent: [this.brokerToEdit.commissionPercent || 0, [Validators.required, Validators.min(0)]]
      });
    }
  }

  saveBroker(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
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
          error: (err) => this.errorMessage = `BROKERS.${err.error.code}`
        });
    }
        else {
        this.brokersService.update(this.brokerToEdit.id, updatedBroker)
          .subscribe({
            next: () => {
              this.successMessage = 'BROKERS.UPDATE_SUCCESS';
              setTimeout(() => this.close.emit(updatedBroker), 800);
            },
            error: () => this.errorMessage = 'BROKERS.UPDATE_ERROR'
          });
      }
  }

  cancel(): void {
    this.close.emit(null);
  }
}
