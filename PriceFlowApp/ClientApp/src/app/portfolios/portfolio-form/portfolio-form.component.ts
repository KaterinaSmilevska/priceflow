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
