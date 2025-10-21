import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Broker {
  id: number;
  kompanija: string;
  procentProvizija: number;
}

@Injectable({
  providedIn: 'root'
})
export class BrokerService {
  private apiUrl = '/api/Broker';

  constructor(private http: HttpClient) { }

  getBroker(kompanija: string): Observable<Broker> {
    const encodedKompanija = encodeURIComponent(kompanija);
    return this.http.get<Broker>(`${this.apiUrl}/${encodedKompanija}`);
  }
}
