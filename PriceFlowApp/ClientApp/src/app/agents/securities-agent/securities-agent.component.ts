import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { SecuritiesAgentService } from './securities-agent.service';
import { AgentMessage } from '../AgentMessage';

@Component({
  selector: 'app-securities-agent',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './securities-agent.component.html',
  styleUrl: './securities-agent.component.css',
})
export class SecuritiesAgentComponent {
  question: string | null = null;
  answer: string | null = null;

  loading = false;
  isOpen = false;

  errorMessage: string | null = null;

  messages: AgentMessage[] = [
    {
      role: 'agent',
      content: 'AGENTS.SECURITIES_AGENT.GREETING'
    }
  ];

  constructor(private securitiesAgentService: SecuritiesAgentService) { }

  toggleAgent(): void {
    this.isOpen = !this.isOpen;
  }

  closeAgent(): void {
    this.isOpen = false;
  }

    askAgent(): void {
      const question = this.question?.trim();

      if(!question || this.loading) {
      return;
      }

      this.messages.push({
        role: 'user',
        content: question
      });

      this.question = '';
      this.loading = true;
      this.errorMessage = null;

      this.securitiesAgentService.askSecuritiesAgent(question).subscribe({
        next: (response) => {
          this.messages.push({
            role: 'agent',
            content: response.answer
          });
          this.loading = false;
        },
        error: (err) => {
          this.errorMessage = err.error?.error || err.error?.message || 'ERRORS.GENERAL_ERROR';
          this.loading = false;
        }
      });
  }

  onEnter(event: Event): void {
    const keyboardEvent = event as KeyboardEvent;
    if (keyboardEvent.shiftKey) {
      return;
    }

    event.preventDefault();
    this.askAgent();
  }
}
