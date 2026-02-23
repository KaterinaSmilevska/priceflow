import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Portfolio, PortfoliosService } from '../portfolios.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-portfolio',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-portfolio.component.html',
  styleUrl: './add-portfolio.component.css',
})
export class AddPortfolioComponent implements OnInit {
  @Input() portfolioToEdit?: Portfolio;
  @Output() close = new EventEmitter<Portfolio | null>();

  form!: FormGroup;

  constructor(private formBuilder: FormBuilder, private portfoliosService: PortfoliosService) { }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: [this.portfolioToEdit?.name || '', Validators.required],
      description: [this.portfolioToEdit?.description || '', Validators.maxLength(100)]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    if (this.portfolioToEdit) {
      this.portfoliosService.updatePortfolio(this.portfolioToEdit.id, this.form.value).subscribe(p =>
        this.close.emit(p));
    } else {
      this.portfoliosService.createPortfolio(this.form.value).subscribe(p => this.close.emit(p));
    }
  }

  onCancel() {
    this.close.emit(null);
  }
}
