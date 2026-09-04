export interface AgentMessage {
  role: 'agent' | 'user';
  content: string;
}
