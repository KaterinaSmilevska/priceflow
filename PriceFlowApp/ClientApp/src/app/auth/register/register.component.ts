import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RegisterService} from './register.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { EmailValidationResponse } from './EmailValidationResponse';
import { PasswordValidationResponse } from './PasswordValidationResponse';
import { RegisterRequest } from './RegisterRequest';
import { RegisterResponse } from './RegisterResponse';
import { UsernameCheckResponse } from './UsernameCheckResponse';

interface Task {
  description: string;
  roleName: string;
  selected: boolean;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  name: string = '';
  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  tasks: Task[] = [
    { description: 'AUTH.SITE_ACTION_1', roleName: 'Инвеститор', selected: false },
    { description: 'AUTH.SITE_ACTION_2', roleName: 'Аналитичар', selected: false },
    { description: 'AUTH.SITE_ACTION_3', roleName: 'Аналитичар', selected: false },
    { description: 'AUTH.SITE_ACTION_4', roleName: 'Обичен корисник', selected: false },
    { description: 'AUTH.SITE_ACTION_5', roleName: 'Обичен корисник', selected: false }
  ];
  roleNames: string[] = [];
  response: RegisterResponse | null = null;
  generalError: string | null = null;
  usernameError: string | null = null;
  emailError: string | null = null;
  passwordError: string | null = null;
  confirmPasswordError: string | null = null;
  isUsernameValid: boolean = true;
  isEmailValid: boolean = true;
  isPasswordValid: boolean = true;
  isEmailVerified: boolean = false;

  constructor(
    private registerService: RegisterService,
    private cdr: ChangeDetectorRef,
    private translateService: TranslateService
  ) { }

  ngOnInit(): void {
    this.checkEmailVerification();
    this.cdr.detectChanges();
  }

  validateUsername(): void {
    if (this.username.trim() === '') {
      this.usernameError = null;
      this.isUsernameValid = false;
      return;
    }
    this.registerService.checkUsername(this.username).subscribe({
      next: (response: UsernameCheckResponse) => {
        this.isUsernameValid = !response.exists;
        this.usernameError = response.exists ? 'USERS.USERNAME_TAKEN' : null;
      },
      error: (err: any) => {
        this.isUsernameValid = true;
        this.usernameError = 'USERS.USERNAME_CHECK_ERROR';
      }
    });
  }

  validateEmail(): void {
    if (this.email.trim() === '') {
      this.emailError = null;
      this.isEmailValid = false;
      return;
    }
    this.registerService.validateEmail({ email: this.email }).subscribe({
      next: (response: EmailValidationResponse) => {
        this.isEmailValid = response.isValid;
        this.emailError = response.isValid ? null : response.message;
      },
      error: (err: any) => {
        this.isEmailValid = true;
        this.emailError = 'USERS.EMAIL_VALIDATION_ERROR';
      }
    });
  }

  validatePassword(): void {
    if (this.password.trim() === '' || this.confirmPassword.trim() === '') {
      this.passwordError = null;
      this.confirmPasswordError = null;
      this.isPasswordValid = false;
      return;
    }
    this.registerService.validatePassword({ password: this.password, confirmPassword: this.confirmPassword }).subscribe({
      next: (response: PasswordValidationResponse) => {
        this.isPasswordValid = response.isValid;
        if (!response.isValid) {
          if (response.message.includes('do not match')) {
            this.passwordError = null;
            this.confirmPasswordError = response.message;
          } else {
            this.passwordError = response.message;
            this.confirmPasswordError = null;
          }
        } else {
          this.passwordError = null;
          this.confirmPasswordError = null;
        }
      },
      error: (err: any) => {
        this.isPasswordValid = true;
        this.passwordError = 'USERS.PASSWORD_VALIDATION_ERROR';
        this.confirmPasswordError = null;
      }
    });
  }

  updateRoleNames(): void {
    const selectedRoles = this.tasks.filter(task => task.selected).map(task => task.roleName);
    this.roleNames = [...new Set(selectedRoles)];
  }

  isFormValid(): boolean {
    return this.name.trim() !== '' &&
      this.username.trim() !== '' &&
      this.email.trim() !== '' &&
      this.password.trim() !== '' &&
      this.confirmPassword.trim() !== '' &&
      this.roleNames.length > 0 &&
      this.isEmailValid &&
      this.isPasswordValid;
  }

  register(): void {
    if (!this.isFormValid()) {
      this.generalError = 'Please fill all fields correctly and select at least one task.';
      return;
    }

    const request: RegisterRequest = {
      name: this.name,
      username: this.username,
      email: this.email,
      password: this.password,
      confirmPassword: this.confirmPassword,
      roleNames: this.roleNames
    };

    this.registerService.register(request).subscribe({
      next: (response: RegisterResponse) => {
        this.response = response;
        this.generalError = null;
        
        this.resetForm();
      },
      error: (err: any) => {
        this.response = null;
        this.generalError = err.error?.message || 'AUTH.REGISTER_ERROR';
      }
    });
  }

  resetForm(): void {
    this.name = '';
    this.username = '';
    this.email = '';
    this.password = '';
    this.confirmPassword = '';
    this.tasks.forEach(task => task.selected = false);
    this.roleNames = [];
    this.usernameError = null;
    this.emailError = null;
    this.passwordError = null;
    this.confirmPasswordError = null;
    this.isUsernameValid = true;
    this.isEmailValid = true;
    this.isPasswordValid = true;
  }

  clearSuccessMessage(): void {
    this.response = null;
    this.resetForm();
  }

  checkEmailVerification(): void {
    const urlParams = new URLSearchParams(window.location.search);
    const verified = urlParams.get('verified');
    if (verified === 'true') {
      this.isEmailVerified = true;
      this.generalError = null;
      window.history.replaceState({}, document.title, window.location.pathname);
    }
  }
}
