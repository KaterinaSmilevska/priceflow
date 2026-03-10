import { Component, EventEmitter, Input, OnInit, Output, } from '@angular/core';
import { SecuritiesService } from '../securities.service';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { Security } from '../Security';
import { TypeSecurity } from '../TypeSecurity';
import { CreateSecurity } from '../CreateSecurity';
import { Issuer } from '../Issuer';

@Component({
  selector: 'app-add-security',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './add-security.component.html',
  styleUrls: ['./add-security.component.css']
})

export class AddSecurityComponent implements OnInit {
  @Input() securityToEdit?: Security;
  @Output() close = new EventEmitter<Security | null>();

  addEditForm!: FormGroup;
  types: TypeSecurity[] = [];
  issuers: Issuer[] = [];

  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(private fb: FormBuilder, private securitiesService: SecuritiesService, private router: Router) { }

  ngOnInit(): void {
    this.addEditForm = this.fb.group({
      code: [this.securityToEdit?.code || '', Validators.required],
      isin: [this.securityToEdit?.isin || '', Validators.required],
      totalNumShares: [this.securityToEdit?.totalNumShares || 0, [Validators.required, Validators.min(1)]],
      typeSecurityId: [0, Validators.required],
      issuerId: [0, Validators.required]
    });

    this.loadTypes();
    this.loadIssuers();
}

loadTypes(): void {
  this.securitiesService.getTypes().subscribe(types => {
    this.types = types;
    if (this.securityToEdit) {
      const type = this.types.find(t => t.name === this.securityToEdit?.typeSecurityName);
      if (type)
        this.addEditForm.patchValue({ typeSecurityId: type.id });
    }
  });
}

loadIssuers(): void {
  this.securitiesService.getIssuers().subscribe(issuers => {
    this.issuers = issuers;
    if (this.securityToEdit) {
      const issuer = this.issuers.find(i => i.name === this.securityToEdit?.issuerName);
      if (issuer)
        this.addEditForm.patchValue({ issuerId: issuer.id });
    }
  });
}

  onSubmit() {
    this.errorMessage = null;
    this.successMessage = null;

    if (this.addEditForm.invalid) return;

    const security: CreateSecurity = this.addEditForm.value;

    if (this.securityToEdit) {
      this.securitiesService.updateSecurity(this.securityToEdit.id, security).subscribe({
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
      this.securitiesService.addSecurity(security).subscribe({
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
