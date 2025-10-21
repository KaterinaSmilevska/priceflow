import {Component, OnInit} from '@angular/core';
import { LoginService } from '../auth/login/login.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  showLoginBubble = false;
  isLoggedIn = false;
  isExpanded = false;

  constructor(public loginService: LoginService) { }

   ngOnInit(): void {
     this.loginService.isLoggedIn().subscribe(isLogged => {
       if (isLogged) {
         this.showLoginSuccess();
       }
     });
   }
  

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  toggleLanguage() {
    const langToggle = document.getElementById('languageToggle') as HTMLInputElement;
    const isMacedonian = langToggle.checked;
    console.log('Language toggled to:', isMacedonian ? 'Macedonian' : 'English');
  }

  openLoginModal() {
    window.location.href = '/login';
  }

  private showLoginSuccess() {
    this.showLoginBubble = true;
    setTimeout(() => {
      this.showLoginBubble = false;
    }, 3000);
  }
}
