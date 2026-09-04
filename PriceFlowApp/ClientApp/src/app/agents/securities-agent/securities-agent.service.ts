import { Injectable } from "@angular/core";
import { ChatResponse } from "../ChatResponse";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ChatRequest } from "../ChatRequest";

@Injectable({
  providedIn: 'root'
})

export class SecuritiesAgentService {
  private apiUrl = "api/securities/agent";

  constructor(private http: HttpClient) { }

  askSecuritiesAgent(question: string): Observable<ChatResponse> {
    const request: ChatRequest = {
      question: question
    };

    return this.http.post<ChatResponse>(this.apiUrl, request);
  }
}
