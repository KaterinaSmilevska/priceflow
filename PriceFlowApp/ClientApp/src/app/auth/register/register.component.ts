import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { RegisterService, RegisterRequest, RegisterResponse, UsernameCheckResponse, PasswordValidationResponse, EmailValidationResponse } from './register.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

interface Task {
  description: string;
  roleName: string;
  selected: boolean;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
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
    { description: 'Manage portfolios and transactions', roleName: 'Инвеститор', selected: false },
    { description: 'Analyze data trends', roleName: 'Аналитичар', selected: false },
    { description: 'Filter and compare current and historic data trends', roleName: 'Аналитичар', selected: false },
    { description: 'View current and historic data trends', roleName: 'Обичен корисник', selected: false },
    { description: 'Do basic filtering and limited searching', roleName: 'Обичен корисник', selected: false }
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
    private cdr: ChangeDetectorRef
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
        this.usernameError = response.exists ? 'Username already exists' : null;
      },
      error: (err: any) => {
        console.error('Username check failed:', err);
        this.isUsernameValid = true;
        this.usernameError = null;
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
        console.error('Email validation failed:', err);
        this.isEmailValid = true;
        this.emailError = null;
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
        console.error('Password validation failed:', err);
        this.isPasswordValid = true;
        this.passwordError = null;
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
        console.error('Registration error: ', err);
        this.response = null;
        this.generalError = err.error?.message || 'Registration failed';
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
