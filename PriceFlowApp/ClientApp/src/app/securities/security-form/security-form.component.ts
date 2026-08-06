import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, } from '@angular/core';
import { SecuritiesService } from '../securities.service';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { Security } from '../Security';
import { TypeSecurity } from '../TypeSecurity';
import { CreateSecurity } from '../CreateSecurity';
import { Issuer } from '../Issuer';
import { UpdateSecurity } from '../UpdateSecurity';

@Component({
  selector: 'app-security-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './security-form.component.html',
  styleUrls: ['./security-form.component.css']
})

export class SecurityFormComponent implements OnInit, OnChanges {
  @Input() securityToEdit?: Security;
  @Output() close = new EventEmitter<Security | null>();

  form!: FormGroup;
  types: TypeSecurity[] = [];
  issuers: Issuer[] = [];

  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(private fb: FormBuilder, private securitiesService: SecuritiesService, private router: Router) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      code: [this.securityToEdit?.code || '', Validators.required],
      isin: [this.securityToEdit?.isin || '', Validators.required],
      totalNumShares: [this.securityToEdit?.totalNumShares || 0, [Validators.required, Validators.min(1)]],
      typeSecurityId: [null, Validators.required],
      issuerId: [null, Validators.required]
    });

    this.loadTypes();
    this.loadIssuers();
    this.loadSecurityData();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['securityToEdit'] && this.form) {
      this.loadSecurityData();
    }
  }

loadTypes(): void {
  this.securitiesService.getTypes().subscribe(types => {
    this.types = types;
    if (this.securityToEdit) {
      const type = this.types.find(t => t.name === this.securityToEdit?.typeSecurityName);
      if (type) {
        this.form.patchValue({
          typeSecurityId: type.id
        });
      }
    }
  });
}

loadIssuers(): void {
  this.securitiesService.getIssuers().subscribe(issuers => {
    this.issuers = issuers;
    if (this.securityToEdit) {
      const issuer = this.issuers.find(i => i.name === this.securityToEdit?.issuerName);
      if (issuer) {
        this.form.patchValue({
          issuerId: issuer.id
        });
      }
    }
  });
  }

  loadSecurityData(): void {
    if (!this.form || !this.securityToEdit) {
      return;
    }

    if (this.securityToEdit) {
      this.form.patchValue({
        code: this.securityToEdit.code,
        isin: this.securityToEdit.isin,
        totalNumShares: this.securityToEdit.totalNumShares
      });

      const type = this.types.find(
        t => t.name === this.securityToEdit?.typeSecurityName
      );

      if (type) {
        this.form.patchValue({
          typeSecurityId: type.id
        });
      }

      const issuer = this.issuers.find(
        i => i.name === this.securityToEdit?.issuerName
      );

      if (issuer) {
        this.form.patchValue({
          issuerId: issuer.id
        });
      }
    }
    else {
      this.form.reset({
        code: '',
        isin: '',
        totalNumShares: 0,
        typeSecurityId: null,
        issuerId: null
      });
    }
  }

  onSubmit() {
    this.errorMessage = null;
    this.successMessage = null;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const updatedSecurity: UpdateSecurity = {
      ...this.securityToEdit,
      ...this.form.value
    };

    if (this.securityToEdit) {
      this.securitiesService.update(this.securityToEdit.id, updatedSecurity).subscribe({
        next: (updated) => {
          this.successMessage = 'SECURITIES.UPDATE_SUCCESS';
          setTimeout(() => this.close.emit(updated), 1000);
        },
        error: (err) => {
          this.errorMessage = 'SECURITIES.UPDATE_ERROR';
          setTimeout(() => this.close.emit(err), 1000);
        }
       });
    } else {
      this.securitiesService.add(this.form.value).subscribe({
        next: (newSecurity) => {
          this.successMessage = 'SECURITIES.ADD_SUCCESS';
          setTimeout(() => this.close.emit(newSecurity), 1000);
        },
        error: (err) => {
          this.errorMessage = 'SECURITIES.ADD_ERROR';
          setTimeout(() => this.close.emit(err), 1000);
        }
      });
    }
  }

  onCancel() {
    this.close.emit(null);
  }
}
