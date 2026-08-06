import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Broker } from './Broker';

@Injectable({
  providedIn: 'root'
})
export class BrokersService {
  private apiUrl = '/api/brokers';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Broker[]> {
    return this.http.get<Broker[]>(this.apiUrl, { withCredentials: true });
  }

  add(broker: Broker): Observable<Broker> {
    return this.http.post<Broker>(`${this.apiUrl}`, broker);
  }

  update(id: number, broker: Broker): Observable<Broker> {
    return this.http.put<Broker>(`${this.apiUrl}/${id}`, broker);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
