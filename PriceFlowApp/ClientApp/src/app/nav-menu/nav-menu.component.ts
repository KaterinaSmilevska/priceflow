import {Component, OnInit} from '@angular/core';
import { LoginService } from '../auth/login/login.service';
import { Observable, map } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css', '../../styles.css']
})
export class NavMenuComponent implements OnInit {
  successMessage: string | null = null;
  isLoggedIn = false;
  isExpanded = false;
  isAdmin$: Observable<boolean>
  private previousLoginState = false;

  constructor(public loginService: LoginService) {
    this.isAdmin$ = this.loginService.getUserRoles().pipe(
      map(roles => roles.includes('Администратор')));
  }

   ngOnInit(): void {
     this.loginService.isLoggedIn().subscribe(currentState => {
       if (this.previousLoginState != currentState) {
         if (currentState) {
           this.showSuccessMessage('Logged in successfully!');
         } else if (this.previousLoginState) {
           this.showSuccessMessage('Logged out successfully!');
         }
       }
       this.previousLoginState = currentState;
       this.isLoggedIn = currentState;
     });
   }

  isAdmin(): boolean {
    return this.loginService.hasRole('Администратор');
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

  private showSuccessMessage(message: string) {
    this.successMessage = message;
    setTimeout(() => {
      this.successMessage = null;
    }, 3000);
  }
}
