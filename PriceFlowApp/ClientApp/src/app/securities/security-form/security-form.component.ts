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

  codeError: string | null = null;
  isinError: string | null = null;
  totalSharesError: string | null = null;

  constructor(private fb: FormBuilder, private securitiesService: SecuritiesService, private router: Router) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      code: [this.securityToEdit?.code || '', Validators.required],
      isin: [this.securityToEdit?.isin || '', [Validators.required, Validators.maxLength(12)]],
      totalNumShares: [this.securityToEdit?.totalNumShares || 0, [Validators.required, Validators.min(1)]],
      typeSecurityId: [null, Validators.required],
      issuerId: [null, Validators.required]
    });

    this.form.get('code')?.valueChanges.subscribe(() => {
      this.validateCode();
    });

    this.form.get('isin')?.valueChanges.subscribe(() => {
      this.validateIsin();
    });

    this.form.get('totalNumShares')?.valueChanges.subscribe(() => {
      this.validateTotalShares();
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

  validateCode(): void {
    this.errorMessage = null;

    const code = this.form.get('code')?.value?.trim() || '';
    if (code === '') {
      this.codeError = 'ERRORS.CODE_VALIDATION_REQUIRED';
    } else {
      this.codeError = null;
    }
  }

  validateIsin(): void {
    this.errorMessage = null;

    const isin = this.form.get('isin')?.value?.trim() || '';
    if (isin === '') {
      this.isinError = 'ERRORS.ISIN_VALIDATION_REQUIRED';
      return;
    }

    if (isin.length > 12) {
      this.isinError = 'ERRORS.INVALID_ISIN';
      return;
    }

    this.isinError = null;
  }

  validateTotalShares(): void {
    this.errorMessage = null;

    const totalShares = this.form.get('totalNumShares')?.value;
    if (totalShares === null || totalShares === undefined || totalShares === '') {
      this.totalSharesError = 'ERRORS.TOTALSHARES_VALIDATION_REQUIRED';

      return;
    }

    if (Number(totalShares) < 1) {
      this.totalSharesError = 'ERRORS.TOTALSHARES_INVALID';

      return;
    }
    this.totalSharesError = null;
  }

  isFormValid(): boolean {
    const code = this.form.get('code')?.value?.trim() || '';
    const isin = this.form.get('isin')?.value?.trim() || '';
    const totalNumShares = this.form.get('totalNumShares')?.value;

    return code !== '' &&
      isin !== '' &&
      totalNumShares !== null &&
      totalNumShares !== undefined &&
      totalNumShares !== '' &&
      Number(totalNumShares) >= 1 &&
      !this.codeError &&
      !this.isinError &&
      !this.totalSharesError;
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

    this.validateCode();
    this.validateIsin();
    this.validateTotalShares();

    if (!this.isFormValid()) {
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
          this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
            : 'SECURITIES.UPDATE_ERROR';
        }
       });
    } else {
      this.securitiesService.add(this.form.value).subscribe({
        next: (newSecurity) => {
          this.successMessage = 'SECURITIES.ADD_SUCCESS';
          setTimeout(() => this.close.emit(newSecurity), 1000);
        },
        error: (err) => {
          this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}`
          : 'SECURITIES.ADD_ERROR';
        }
      });
    }
  }

  onCancel() {
    this.close.emit(null);
  }
}
