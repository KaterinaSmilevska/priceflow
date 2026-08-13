import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [TranslateModule],
  templateUrl: './server-error.component.html',
  styleUrl: './server-error.component.css',
})
export class ServerErrorComponent {
  constructor(private router: Router) { }

  redirectToHome(): void {
    this.router.navigate(['/']);
  }
}
