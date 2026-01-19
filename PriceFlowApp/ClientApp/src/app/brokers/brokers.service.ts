import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Broker {
  id: number;
  company: string;
  commissionPercent: number;
}

@Injectable({
  providedIn: 'root'
})
export class BrokersService {
  private apiUrl = '/api/brokers';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Broker[]> {
    return this.http.get<Broker[]>(this.apiUrl, { withCredentials: true });
  }
}
