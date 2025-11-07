import { Component, EventEmitter, Input, OnInit, Output, } from '@angular/core';
import { CreateSecurity, Issuer, SecuritiesService, Security, TypeSecurity } from '../securities.service';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-security',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule],
  templateUrl: './add-security.component.html',
  styleUrls: ['./add-security.component.css']
})

export class AddSecurityComponent implements OnInit {
  @Input() securityToEdit?: Security;
  @Output() close = new EventEmitter<Security | null>();

  addEditForm!: FormGroup;
  types: TypeSecurity[] = [];
  issuers: Issuer[] = [];

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
    if (this.addEditForm.invalid) return;

    const security: CreateSecurity = this.addEditForm.value;

    if (this.securityToEdit) {
      this.securitiesService.updateSecurity(this.securityToEdit.id, security).subscribe(
        updated => {
          this.close.emit(updated);
        });
      ;
    } else {
      this.securitiesService.addSecurity(security).subscribe(newSecurity => {
        this.close.emit(newSecurity);
      });
    }
  }

  onCancel() {
    this.close.emit(null);
  }
}
