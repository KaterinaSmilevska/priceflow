import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SecuritiesService } from '../../securities/securities.service';
import { PortfolioReturnsService } from './portfolio-returns.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { PortfolioReturnsSummary } from './PortfolioReturnsSummary';
import { Security } from '../../securities/Security';

@Component({
  selector: 'app-portfolio-returns',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './portfolio-returns.component.html',
  styleUrl: './portfolio-returns.component.css',
})
export class PortfolioReturnsComponent implements OnInit {
  @Input() portfolioId!: number;
  @Output() updated = new EventEmitter<void>();

  form!: FormGroup;
  summary?: PortfolioReturnsSummary;
  securities: Security[] = [];

  constructor(private fb: FormBuilder, private portfolioReturnsService: PortfolioReturnsService,
    private securitiesService: SecuritiesService) { }

  ngOnInit() {
    this.form = this.fb.group({
      date: [new Date().toISOString().substring(0, 10), Validators.required],
      securityId: [null, Validators.required],
      netAmount: [0, [Validators.required, Validators.min(0)]],
      tax: [0, [Validators.required, Validators.min(0)]]
    });

    this.loadSummary();
    this.loadSecurities();
  }

  save(): void {
    if (this.form.invalid) return;

    const payload = {
      ...this.form.value,
      portfolioId: this.portfolioId
    };

    this.portfolioReturnsService.add(payload)
      .subscribe(() => {
        this.loadSummary();
        this.updated.emit();
        this.form.patchValue({ netAmount: 0, tax: 0 })
      });
  }

  loadSummary(): void {
    this.portfolioReturnsService.getSummary(this.portfolioId)
      .subscribe(s => this.summary = s);
  }

  loadSecurities(): void {
    this.securitiesService.getAll().subscribe(s => this.securities = s);
  }
}
