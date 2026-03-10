import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterRequest } from './RegisterRequest';
import { RegisterResponse } from './RegisterResponse';
import { UsernameCheckResponse } from './UsernameCheckResponse';
import { PasswordValidationRequest } from './PasswordValidationRequest';
import { PasswordValidationResponse } from './PasswordValidationResponse';
import { EmailValidationRequest } from './EmailValidationRequest';
import { EmailValidationResponse } from './EmailValidationResponse';

@Injectable({
  providedIn: 'root'
})

export class RegisterService {
  private apiUrl = '/api/auth';

  constructor(private http: HttpClient) { }

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, request);
  }

  checkUsername(username: string): Observable<UsernameCheckResponse> {
    return this.http.get<UsernameCheckResponse>(`${this.apiUrl}/check-username/${encodeURIComponent(username)}`);
  }

  validatePassword(request: PasswordValidationRequest): Observable<PasswordValidationResponse> {
    return this.http.post<PasswordValidationResponse>(`${this.apiUrl}/validate-password`, request);
  }

  validateEmail(request: EmailValidationRequest): Observable<EmailValidationResponse> {
    return this.http.post<EmailValidationResponse>(`${this.apiUrl}/validate-email`, request);
  }

  verifyEmail(token: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/verify-email?token=${token}`);
  }
}
