import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { LoginService } from './login.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { LoginRequest } from './LoginRequest';
import { LoginResponse } from './LoginResponse';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent {
  username: string = '';
  password: string = '';

  usernameError: string | null = null;
  passwordError: string | null = null;
  errorMessage: string | null = null;
  roles: string[] = [];

  constructor(public loginService: LoginService, private router: Router) { }

  validateUsername(): void {
    this.errorMessage = null;

    const username = this.username.trim();
    if (username === '') {
      this.usernameError = 'ERRORS.USERNAME_VALIDATION_REQUIRED';
    } else {
      this.usernameError = null;
    }
  }

  validatePassword(): void {
    this.errorMessage = null;

    const password = this.password.trim();
    if (password === '') {
      this.passwordError = 'ERRORS.PASSWORD_VALIDATION_REQUIRED';
    } else {
      this.passwordError = null;
    }
  }

  isFormValid(): boolean {
    return this.username.trim() !== '' &&
      this.password.trim() !== '';
  }

  onLogin(): void {
    this.validateUsername();
    this.validatePassword();

    if (!this.isFormValid()) {
      return;
    }

    const request: LoginRequest = { username: this.username, password: this.password };
    this.loginService.login(request).subscribe({
      next: (response: LoginResponse) => {
        this.roles = response.roles || [];
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.errorMessage = err.error?.code
          ? `ERRORS.${err.error.code}`
          : 'ERRORS.LOGIN_ERROR';
      }
    });
  }
}
