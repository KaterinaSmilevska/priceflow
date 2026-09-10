import { Injectable } from "@angular/core";
import { ChatResponse } from "../ChatResponse";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ChatRequest } from "../ChatRequest";
import { AgentConversation } from "../AgentConversation";
import { AgentMessageResponse } from "../AgentMessageResponse";

@Injectable({
  providedIn: 'root'
})

export class SecuritiesAgentService {
  private apiUrl = "api/securities/agent";
  private agentUrl = "api/agent/conversations"

  constructor(private http: HttpClient) { }

  getConversations(): Observable<AgentConversation[]> {
    return this.http.get<AgentConversation[]>(this.agentUrl);
  }

  getConversationMessages(conversationId: number): Observable<AgentMessageResponse[]> {
    return this.http.get<AgentMessageResponse[]>(`${this.agentUrl}/${conversationId}/messages`);
  }

  askSecuritiesAgent(conversationId: number | null, question: string): Observable<ChatResponse> {
    const request: ChatRequest = {
      conversationId: conversationId,
      question: question
    };

    return this.http.post<ChatResponse>(this.apiUrl, request);
  }
}
