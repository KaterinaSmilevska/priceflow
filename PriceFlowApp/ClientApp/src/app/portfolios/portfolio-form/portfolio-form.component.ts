import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PortfoliosService } from '../portfolios.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { Portfolio } from '../Portfolio';

@Component({
  selector: 'app-portfolio-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './portfolio-form.component.html',
  styleUrl: './portfolio-form.component.css',
})
export class PortfolioFormComponent implements OnInit {
  @Input() portfolioToEdit?: Portfolio;
  @Output() close = new EventEmitter<Portfolio | null>();

  form!: FormGroup;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(private formBuilder: FormBuilder, private portfoliosService: PortfoliosService) { }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: [this.portfolioToEdit?.name || '', Validators.required],
      description: [this.portfolioToEdit?.description || '', Validators.maxLength(100)]
    });
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    };

    const updatedPortfolio: Portfolio = { ...this.portfolioToEdit, ...this.form.value };

    if (!this.portfolioToEdit) {
      this.portfoliosService.add(this.form.value)
        .subscribe({
          next: (b) => {
            this.successMessage = 'PORTFOLIOS.ADD_SUCCESS';
            setTimeout(() => this.close.emit(b), 800)
          },
          error: () => this.errorMessage = 'PORTFOLIOS.ADD_ERROR'
        });
    } else {
      this.portfoliosService.update(this.portfolioToEdit.id, updatedPortfolio)
        .subscribe({
          next: () => {
            this.successMessage = 'PORTFOLIOS.UPDATE_SUCCESS';
            setTimeout(() => this.close.emit(updatedPortfolio), 800)
          },
          error: () => this.errorMessage = 'PORTFOLIOS.UPDATE_ERROR'
        });
    }
  }

  onCancel() {
    this.close.emit(null);
  }
}
