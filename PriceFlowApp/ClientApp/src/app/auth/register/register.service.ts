import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface RegisterRequest {
  ime: string;
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  ulogaNames: string[];
}

export interface RegisterResponse {
  id: number;
  username: string;
  email: string;
  ulogas: string[];
  message: string;
}

export interface UsernameCheckResponse {
  exists: boolean;
}

export interface PasswordValidationRequest {
  password: string;
  confirmPassword: string;
}

export interface PasswordValidationResponse {
  isValid: boolean;
  message: string;
}

export interface EmailValidationRequest {
  email: string;
}

export interface EmailValidationResponse {
  isValid: boolean;
  message: string;
}

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
}
