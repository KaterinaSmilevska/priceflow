import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  id: number;
  username: string;
  email: string;
  ulogas: string[];
  message: string;
}

@Injectable({
  providedIn: 'root'
})

export class LoginService {
  private apiUrl = '/api/auth';
  private loggedIn = new BehaviorSubject<boolean>(false);
  private username = new BehaviorSubject<string | null>(null);

  constructor(private http: HttpClient) {
    this.http.get<{ isLoggedIn: boolean; username?: string }>(`${this.apiUrl}/status`)
      .subscribe(status => {
        this.loggedIn.next(status.isLoggedIn);
        this.username.next(status.username || '');
      });
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request, { withCredentials: true }).pipe(
      tap(response => {
        if (response && response.message === 'Login successful') {
          this.loggedIn.next(true);
          this.username.next(response.username);
        }
      })
    );
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/logout`, {}, {withCredentials: true}).subscribe(() => {
      this.loggedIn.next(false);
      this.username.next(null);
    });
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  getUsername(): Observable<string | null> {
    return this.username.asObservable();
  }
}
