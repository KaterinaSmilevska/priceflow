import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { PortfoliosService } from '../portfolios.service';
import { PortfolioPerformanceSummary } from './PortfolioPerformanceSummary';

@Component({
  selector: 'app-performance-export',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './performance-export.component.html',
  styleUrl: './performance-export.component.css',
})
export class PerformanceExportComponent implements OnInit {
  @Input() portfolioId!: number;

  form!: FormGroup;
  summary?: PortfolioPerformanceSummary;
  loading = false;
  exporting = false;
  errorMessage: string | null = null;

  constructor(private fb: FormBuilder, private portfoliosService: PortfoliosService) { }

  ngOnInit() {
    this.form = this.fb.group({
      from: ['', Validators.required],
      to: ['', Validators.required],
      format: ['pdf', Validators.required]
    },
      { validators: this.dateRangeValidator }
    );
  }

  dateRangeValidator(group: FormGroup) {
    const from = group.get('from')?.value;
    const to = group.get('to')?.value;

    if (from && to && new Date(from) > new Date(to)) {
      return { dateInvalid: true };
    }
    return null;
  }

  fetchSummary() {
    if (this.form.invalid) return;

    this.loading = true;
    this.errorMessage = '';
    const from = this.form.get('from')?.value;
    const to = this.form.get('to')?.value;

    this.portfoliosService.getPerformanceSummary(this.portfolioId, from, to)
      .subscribe({
        next: (res) => {
          this.summary = res;
          this.loading = false;
        },
        error: () => {
          this.errorMessage = 'Error fetching summary';
          this.loading = false;
        }
      });
    this.form.valueChanges.subscribe(() => this.summary = undefined);
  }

  export() {
    if (this.form.invalid) return;

    this.exporting = true;

    const { from, to, format } = this.form.value;

    this.portfoliosService.exportPerformanceSummary(this.portfolioId, from, to, format)
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');

          const extension = format === 'excel' ? 'xlsx' : format;

          a.href = url;
          a.download = `portfolio_performance_summary.${extension}`;

          a.click();

          window.URL.revokeObjectURL(url);
          this.exporting = false;
        },
        error: () => {
          this.errorMessage = 'Error exporting report.';
          this.exporting = false;
        }
      });
    }
}
