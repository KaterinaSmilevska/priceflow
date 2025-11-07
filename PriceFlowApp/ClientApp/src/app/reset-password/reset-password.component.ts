import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  username: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  resetToken: string | null = null;
  errorMessage: string = '';

  constructor(private route: ActivatedRoute, private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.resetToken = this.route.snapshot.queryParamMap.get('token');
  }

  sendResetLink(): void {
    this.http.post('/api/auth/forgot-password', { username: this.username }).subscribe({
      next: () => {
        this.errorMessage = 'Reset link has been sent to your email.';
      },
      error: (err) => {
        this.errorMessage = err.error.message || 'Failed to send reset link.';
      }
    });
  }

  resetPassword(): void {
    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }
    this.http.post('/api/auth/reset-password', { token: this.resetToken, newPassword: this.newPassword }).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage = err.error.message || 'Failed to reset password.';
      }
    });
  }
}
