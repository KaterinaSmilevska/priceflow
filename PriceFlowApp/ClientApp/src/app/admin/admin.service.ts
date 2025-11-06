import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface User {
  id: number;
  name: string;
  username: string;
  email: string;
  roles: string[]
}

export interface AuthStatus {
  isLoggedIn: boolean;
  username?: string;
  userId?: number;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = '/api/auth';

  constructor(private http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`);
  }

  getUser(userId: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/users/${userId}`);
  }

  updateUser(user: User): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${user.id}`, user);
  }

  deleteUser(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/users/${userId}`);
  }

  getCurrentUserStatus(): Observable<AuthStatus> {
    return this.http.get<AuthStatus>(`${this.apiUrl}/status`);
  }

  checkUsername(username: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-username/${username}`);
  }
}
