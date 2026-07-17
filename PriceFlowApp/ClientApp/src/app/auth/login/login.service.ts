import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { LoginRequest } from './LoginRequest';
import { LoginResponse } from './LoginResponse';
import { filter, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class LoginService {
  private apiUrl = '/api/auth';
  private loggedIn = new BehaviorSubject<boolean | null>(null);
  private username = new BehaviorSubject<string | null>(null);
  private userRoles = new BehaviorSubject<string[]>([]);

  private authLoaded = new BehaviorSubject<boolean>(false);
  
  constructor(private http: HttpClient, private router: Router) {
    this.http.get<{ isLoggedIn: boolean; username?: string, roles?: string[] }>(`${this.apiUrl}/status`, { withCredentials: true })
      .subscribe(status => {
        this.loggedIn.next(status.isLoggedIn);
        this.username.next(status.username || '');
        this.userRoles.next(status.roles || []);

        this.authLoaded.next(true);
      });
  }

  authReady(): Observable<boolean> {
    return this.authLoaded.asObservable().pipe(
      filter(ready => ready),
      take(1)
    );
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request, { withCredentials: true }).pipe(
      tap(response => {
        if (response && response.message === 'Login successful') {
          this.loggedIn.next(true);
          this.username.next(response.username);
          this.userRoles.next(response.roles);

          this.authLoaded.next(true);
        }
      })
    );
  }

  logout(): void {
    this.http.post(`${this.apiUrl}/logout`, {}, {withCredentials: true}).subscribe(() => {
      this.loggedIn.next(false);
      this.username.next(null);
      this.userRoles.next([]);

      this.router.navigate(['/login']);
    });
  }

  isLoggedIn(): Observable<boolean> {
    return this.loggedIn.asObservable().pipe(
      filter((value): value is boolean => value !== null)
    );
  }

  getUsername(): Observable<string | null> {
    return this.username.asObservable();
  }

  getUserRoles(): Observable<string[]> {
    return this.userRoles.asObservable();
  }

  hasRole(role: string): boolean {
    return this.userRoles.value.includes(role);
  }

  verifyEmail(token: string) {
    return this.http.get(`${this.apiUrl}/verify-email?token=${encodeURIComponent(token)}`);
  }

  getSession(): Observable<any> {
    return this.http.get<any>('api/auth/session-test', { withCredentials: true });
  }

  isInvestor(): Observable<boolean> {
    return this.userRoles.asObservable().pipe(
      map(roles => roles.includes('Инвеститор'))
    );
  }

  isAnalyst(): Observable<boolean> {
    return this.userRoles.asObservable().pipe(
      map(roles => roles.includes('Аналитичар'))
    );
  }
}
