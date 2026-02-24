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

  addThreshold(request: CreateThresholdRequest) {
    return this.http.post(this.apiUrl, request);
  }

  updateThreshold(id: number, request: UpdateThresholdRequest) {
    return this.http.put(`${this.apiUrl}/${id}`, request);
  }

  deleteThreshold(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

}
