import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoginService } from '../login/login.service';

@Component({
  selector: 'app-verify-email',
  template:
    `
    <div class="verify-container">
     <h2>{{ message }}</h2>
    </div>
    `
})

export class VerifyEmailComponent implements OnInit {
  message = 'Verifying your email...';

  constructor(private route: ActivatedRoute, private loginService: LoginService) { }

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');

    if (token) {
      this.loginService.verifyEmail(token).subscribe({
        next: () => this.message = 'Your email has been verified successfully!',
        error: () => this.message = 'Invalid or expired verification link.'
      });
    }
  }
}
