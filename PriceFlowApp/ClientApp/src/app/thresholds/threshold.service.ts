import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Threshold } from "./Threshold";
import { OwnedSecurity } from "./OwnedSecurity";
import { CreateThresholdRequest } from "./CreateThresholdRequest";
import { UpdateThresholdRequest } from "./UpdateThresholdRequest";

@Injectable({
  providedIn: 'root'
})

export class ThresholdService {
  private apiUrl = 'api/threshold';

  constructor(private http: HttpClient) { }

  getOwned(): Observable<OwnedSecurity[]> {
    return this.http.get<OwnedSecurity[]>(`${this.apiUrl}/owned`)
  }

  getUserThresholds(): Observable<Threshold[]> {
    return this.http.get<Threshold[]>(this.apiUrl);
  }

  add(request: CreateThresholdRequest) {
    return this.http.post(this.apiUrl, request);
  }

  update(id: number, request: UpdateThresholdRequest) {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<Threshold> {
    return this.http.delete<Threshold>(`${this.apiUrl}/${id}`);
  }
}
