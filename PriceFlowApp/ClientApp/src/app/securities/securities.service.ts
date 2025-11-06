import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Security {
  id: number;
  isin: string;
  code: string;
  typeSecurityName: string;
  issuerName: string;
  totalNumShares: number;
}

@Injectable({
  providedIn: 'root'
})

export class SecuritiesService {
  private apiUrl = "api/securities";

  constructor(private http: HttpClient) { }

  getAll(): Observable<Security[]> {
    return this.http.get<Security[]>(this.apiUrl);
  }

  getById(id: number): Observable<Security> {
    return this.http.get<Security>(`${this.apiUrl}/${id}`);
  }
}
