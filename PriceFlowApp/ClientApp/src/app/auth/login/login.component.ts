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
  errorMessage: string = '';
  roles: string[] = [];

  constructor(public loginService: LoginService, private router: Router) { }

  onLogin(): void {
    const request: LoginRequest = { username: this.username, password: this.password };
    this.loginService.login(request).subscribe({
      next: (response: LoginResponse) => {
        this.roles = response.roles || [];
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.errorMessage = 'AUTH.INVALID_CREDENTIALS';
      }
    });
  }
}
