import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from './manage-users/User';
import { Broker } from './manage-brokers/Broker';
import { AuthStatus } from './AuthStatus';

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

  getBrokers(): Observable<Broker[]> {
    return this.http.get<Broker[]>(`${this.apiUrl}/brokers`);
  }

  addBroker(broker: Broker): Observable<Broker> {
    return this.http.post<Broker>(`${this.apiUrl}/brokers`, broker);
  } 

  updateBroker(broker: Broker): Observable<Broker> {
    return this.http.put<Broker>(`${this.apiUrl}/brokers/${broker.id}`, broker);
  }

  deleteBroker(brokerId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/brokers/${brokerId}`);
  }

  getCurrentUserStatus(): Observable<AuthStatus> {
    return this.http.get<AuthStatus>(`${this.apiUrl}/status`);
  }

  checkUsername(username: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-username/${username}`);
  }
}
