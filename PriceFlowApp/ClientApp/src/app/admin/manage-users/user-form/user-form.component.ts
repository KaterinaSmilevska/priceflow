import { Component, Input, Output, EventEmitter, OnInit, SimpleChanges, OnChanges } from '@angular/core';
import { AdminService } from '../../admin.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../User';
import { TranslateModule } from '@ngx-translate/core';
import { UsernameCheckResponse } from '../../../auth/register/UsernameCheckResponse';
import { RegisterService } from '../../../auth/register/register.service';
import { EmailValidationResponse } from '../../../auth/register/EmailValidationResponse';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})

export class UserFormComponent implements OnInit, OnChanges {
  @Input() userToEdit!: User;
  @Output() close = new EventEmitter<User | null>();

  form!: FormGroup;

  originalUsername: string | null = null;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  nameError: string | null = null;
  usernameError: string | null = null;
  emailError: string | null = null;

  isUsernameValid = true;
  isEmailValid = true;

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService,
    private registerService: RegisterService
  ) { }

  ngOnInit(): void {

  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['userToEdit'] && this.userToEdit) {
      this.originalUsername = this.userToEdit.username;

      this.form = this.fb.group({
        name: [this.userToEdit.name || '', Validators.required],
        username: [this.userToEdit.username || '', Validators.required],
        email: [this.userToEdit.email || '', [Validators.required, Validators.email]]
      });

      this.form.get('name')?.valueChanges.subscribe(() => {
        this.validateName();
      });

      this.form.get('username')?.valueChanges.subscribe(() => {
        this.validateUsername();
      });

      this.form.get('email')?.valueChanges.subscribe(() => {
        this.validateEmail();
      });
    }
  }

  validateName(): void {
    this.errorMessage = null;

    const name = this.form.get('name')?.value?.trim() || '';
    if (name === '') {
      this.nameError = 'ERRORS.NAME_VALIDATION_REQUIRED';
    } else {
      this.nameError = null;
    }
  }

  validateUsername(): void {
    this.errorMessage = null;
    this.usernameError = null;

    const username = this.form.get('username')?.value?.trim() || '';
    if (username === '') {
      this.usernameError = 'ERRORS.USERNAME_VALIDATION_REQUIRED';
      this.isUsernameValid = false;
      return;
    }

    if (username === this.originalUsername) {
      this.usernameError = null;
      this.isUsernameValid = true;
      return;
    }

    this.adminService.checkUsername(username).subscribe({
      next: (response: UsernameCheckResponse) => {
        this.isUsernameValid = !response.exists;
        this.usernameError = response.exists ? 'ERRORS.USERNAME_ALREADY_EXISTS' : null;
      },
      error: (err) => {
        this.isUsernameValid = true;
        this.usernameError = err.error?.code
          ? `ERRORS.${err.error.code}`
          : 'ERRORS.USERNAME_VALIDATION_REQUIRED';
      }
    });
  }

  validateEmail(): void {
    this.errorMessage = null;

    const email = this.form.get('email')?.value?.trim() || '';
    if (email === '') {
      this.emailError = 'ERRORS.EMAIL_VALIDATION_REQUIRED';
      this.isEmailValid = false;
      return;
    }

    this.registerService.validateEmail({ email: email }).subscribe({
      next: (response: EmailValidationResponse) => {
        this.isEmailValid = response.isValid;

        this.emailError = response.isValid
          ? null
          : `ERRORS.${response.code}`
      },
      error: (err: any) => {
        this.isEmailValid = false;
        this.emailError = err.error?.code
          ? `ERRORS.${err.error.code}`
          : 'ERRORS.EMAIL_VALIDATION_REQUIRED'
      }
    });
  }

  isFormValid(): boolean {
    const name = this.form.get('name')?.value?.trim() || '';
    const username = this.form.get('username')?.value?.trim() || '';
    const email = this.form.get('email')?.value?.trim() || '';

    return name !== '' &&
      username !== '' &&
      email !== '' &&
      this.isUsernameValid &&
      this.isEmailValid &&
      !this.nameError &&
      !this.usernameError &&
      !this.emailError;
  }

  saveUser(): void {
    this.errorMessage = null;
    this.successMessage = null;

    this.validateName();
    this.validateUsername();
    this.validateEmail();

    if (!this.isFormValid()) {
      return;
    }

    const updatedUser: User = {
      ...this.userToEdit,
      ...this.form.value
    }

    this.adminService.updateUser(this.userToEdit.id, updatedUser).subscribe({
          next: () => {
            this.successMessage = 'USERS.UPDATE_SUCCESS';
            setTimeout(() => this.close.emit(updatedUser), 800);
          },
      error: (err) => {
        this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}` : 'USERS.UPDATE_ERROR';
      }
     });
    }

  cancel(): void {
    this.close.emit(null);
  }
}
