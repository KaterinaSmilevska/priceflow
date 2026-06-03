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
  errorMessage: string = '';
  successMessage: string = '';

  constructor(private route: ActivatedRoute, private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.resetToken = this.route.snapshot.queryParamMap.get('token');
  }

  sendResetLink(): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.http.post('/api/auth/forgot-password', { username: this.username }).subscribe({
      next: () => {
        this.successMessage = 'AUTH.RESET_LINK_SUCCESS';
      },
      error: (err) => {
        this.errorMessage = 'AUTH.RESET_LINK_ERROR';
      }
    });
  }

  resetPassword(): void {
    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'AUTH.PASSWORD_MISSMATCH';
      return;
    }
    this.http.post('/api/auth/reset-password', { token: this.resetToken, newPassword: this.newPassword }).subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.errorMessage = err.error.message || 'AUTH.RESET_PASSWORD_ERROR';
      }
    });
  }
}
