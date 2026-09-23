import { CommonModule } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { SecuritiesAgentService } from './securities-agent.service';
import { AgentMessage } from '../AgentMessage';
import { AgentConversation } from '../AgentConversation';
import { LoginService } from '../../auth/login/login.service';

@Component({
  selector: 'app-securities-agent',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './securities-agent.component.html',
  styleUrl: './securities-agent.component.css',
})
export class SecuritiesAgentComponent implements AfterViewChecked, OnInit {
  @ViewChild('agentChat')
  private agentChat!: ElementRef<HTMLDivElement>
  conversationId: number | null = null;

  question: string | null = null;
  answer: string | null = null;

  loading = false;
  isOpen = false;
  dailyLimitReached = false;
  copiedMessageIndex: number | null = null;

  canUseAgent = false;

  private shouldScroll = false;

  conversations: AgentConversation[] = [];
  showConversations = false;
  loadingConversations = false;

  errorMessage: string | null = null;

  messages: AgentMessage[] = [
    {
      role: 'agent',
      content: 'AGENTS.SECURITIES_AGENT.GREETING'
    }
  ];

  constructor(private securitiesAgentService: SecuritiesAgentService, private translateService: TranslateService,
    private loginService: LoginService) { }

  ngOnInit(): void {
    this.loginService.authReady().subscribe(() => {
      this.canUseAgent = this.loginService.canUseAgent();
    });
  }

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.scrollBar();
      this.shouldScroll = false;
    }
  }

  toggleAgent(): void {
    if (!this.canUseAgent) {
      return;
    }

    this.isOpen = !this.isOpen;

    if (this.isOpen) {
      this.loadConversations();
      this.shouldScroll = true;
    }
  }

  closeAgent(): void {
    this.isOpen = false;
  }

  newConversation(): void {
    if (this.loading) {
      return;
    }

    this.conversationId = null;
    this.question = '';
    this.answer = null;
    this.errorMessage = null;
    this.copiedMessageIndex = null;

    this.messages = [
      {
        role: 'agent',
        content: 'AGENTS.SECURITIES_AGENT.GREETING'
      }
    ];

    this.shouldScroll = true;
    this.showConversations = false;
  }

  loadConversations(): void {
    this.loadingConversations = true;

    this.securitiesAgentService.getConversations().subscribe({
      next: (conversations) => {
        this.conversations = conversations;
        this.loadingConversations = false;
      },
      error: () => {
        this.loadingConversations = false;
      }
    });
  }

  openConversation(conversation: AgentConversation): void {
    if (this.loading) {
      return;
    }

    this.loading = true;
    this.errorMessage = null;

    this.securitiesAgentService.getConversationMessages(conversation.id)
      .subscribe({
        next: (messages) => {
          this.conversationId = conversation.id;

          this.messages = messages.map(message => ({
            role: message.role === 'user' ? 'user' : 'agent',
            content: message.content
          }));

          this.showConversations = false;
          this.loading = false;
          this.shouldScroll = true;
        },
        error: (err) => {
          this.errorMessage = err.error?.code ? `AGENTS.${err.error.code}` : 'ERRORS.GENERAL_ERROR';

          this.loading = false;
        }
      });
  }

  deleteConversation(conversation: AgentConversation): void {
    if (this.loading) {
      return;
    }

    this.securitiesAgentService.deleteConversation(conversation.id)
      .subscribe({
        next: () => {
          this.conversations = this.conversations.filter(item => item.id !== conversation.id);

          if (this.conversationId == conversation.id) {
            this.newConversation();
          }
        },
        error: (err) => {
          this.errorMessage = err.error?.code ? `AGENTS.${err.error.code}` : 'ERRORS.GENERAL_ERROR';
        }
      })
  }

  askAgent(): void {
    if (!this.canUseAgent) {
      return;
    }

    const question = this.question?.trim();

    if (!question || this.loading || this.dailyLimitReached) {
      return;
    }

      this.loading = true;
      this.errorMessage = null;

      this.securitiesAgentService.askSecuritiesAgent(this.conversationId, question).subscribe({
        next: (response) => {
          this.messages.push({
            role: 'user',
            content: question
          });

          this.messages.push({
            role: 'agent',
            content: response.answer
          });

          this.conversationId = response.conversationId;

          this.loadConversations();

          this.question = '';
          this.loading = false;
          this.shouldScroll = true;
        },
        error: (err) => {
          if (err.error?.code === 'DAILY_LIMIT_REACHED') {
            this.dailyLimitReached = true;

            this.messages.push({
              role: 'agent',
              content: 'AGENTS.DAILY_LIMIT_REACHED'
            });
            this.shouldScroll = true;
          } else if (err.error?.code === 'MESSAGE_LENGTH_INVALID') {
            this.errorMessage = 'AGENTS.MESSAGE_LENGTH_INVALID';
          } else {
            this.errorMessage = err.error?.code ? `AGENTS.${err.error.code}` : 'ERRORS.GENERAL_ERROR';
          }
          this.loading = false;
        }
      });
  }

  async copyMessage(content: string, index: number): Promise<void> {
    try {
      const translatedContent = this.translateService.instant(content);

      await navigator.clipboard.writeText(translatedContent);

      this.copiedMessageIndex = index;

      setTimeout(() => {
        this.copiedMessageIndex = null;
      }, 1500);
    } catch {
      this.errorMessage = 'ERRORS.COPY_FAILED'
    }
  }

  onEnter(event: Event): void {
    const keyboardEvent = event as KeyboardEvent;
    if (keyboardEvent.shiftKey) {
      return;
    }

    event.preventDefault();
    this.askAgent();
  }

  private scrollBar(): void {
    if (!this.agentChat) {
      return;
    }
    this.agentChat.nativeElement.scrollTop = this.agentChat.nativeElement.scrollHeight;
  }

  scrollToTop(): void {
    if (!this.agentChat) {
      return;
    }

    this.agentChat.nativeElement.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  scrollToBottom(): void {
    if (!this.agentChat) {
      return;
    }

    this.agentChat.nativeElement.scrollTo({
      top: this.agentChat.nativeElement.scrollHeight,
      behavior: 'smooth'
    });
  }
}
