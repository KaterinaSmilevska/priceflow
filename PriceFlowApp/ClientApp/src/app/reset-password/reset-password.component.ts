import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  username: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  resetToken: string | null = null;
  usernameError: string | null = null;
  passwordError: string | null = null;
  confirmPasswordError: string | null = null;
  errorMessage: string | null = null;
  successMessage: string = '';

  constructor(private route: ActivatedRoute, private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.resetToken = this.route.snapshot.queryParamMap.get('token');
  }

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

    const password = this.newPassword.trim();
    if (password === '') {
      this.passwordError = 'ERRORS.PASSWORD_VALIDATION_REQUIRED';
    } else {
      this.passwordError = null;
    }
  }

  validateConfirmPassword(): void {
    this.errorMessage = null;

    const password = this.confirmPassword.trim();
    if (password === '') {
      this.confirmPasswordError = 'ERRORS.PASSWORD_VALIDATION_REQUIRED';
    } else {
      this.confirmPasswordError = null;
    }
  }

  sendResetLink(): void {
    this.validateUsername();
    this.successMessage = '';
    this.errorMessage = '';

    this.http.post('/api/auth/forgot-password', { username: this.username }).subscribe({
      next: () => {
        this.successMessage = 'AUTH.RESET_LINK_SUCCESS';
      },
      error: (err) => {
        this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}` : 'AUTH.RESET_LINK_ERROR';
      }
    });
  }

  resetPassword(): void {
    this.validatePassword();
    this.validateConfirmPassword();

    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'ERRORS.PASSWORD_MISMATCH';
      return;
    }
    this.http.post('/api/auth/reset-password', { token: this.resetToken, newPassword: this.newPassword }).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage = err.error?.code ? `ERRORS.${err.error.code}` : 'AUTH.RESET_PASSWORD_ERROR';
      }
    });
  }
}
