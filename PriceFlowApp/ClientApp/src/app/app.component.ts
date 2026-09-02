import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterOutlet, NavMenuComponent],
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(public translateService: TranslateService) {
    translateService.addLangs(['mk', 'en']);
    translateService.setFallbackLang('mk');

    const browserLang = translateService.getBrowserLang();

    translateService.use(browserLang?.match(/mk|en/) ? browserLang : 'mk');

    const savedLang = localStorage.getItem('lang') || 'mk';
    translateService.use(savedLang);
  }
}
