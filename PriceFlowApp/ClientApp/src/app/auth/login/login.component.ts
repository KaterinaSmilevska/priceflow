import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LoginRequest, LoginResponse, LoginService } from './login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';
  ulogas: string[] = [];

  constructor(public loginService: LoginService, private router: Router) { }

  onLogin(): void {
    const request: LoginRequest = { username: this.username, password: this.password };
    this.loginService.login(request).subscribe({
      next: (response: LoginResponse) => {
        this.ulogas = response.ulogas || [];
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.errorMessage = err.error?.Message || 'Login failed';
      }
    });
  }
}
