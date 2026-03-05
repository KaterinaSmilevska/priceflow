import { Injectable } from "@angular/core";

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private darkMode = false;

  constructor() {
    this.loadPreference();
  }

  isDarkMode(): boolean {
    return this.darkMode;
  }

  toggleDarkMode(): void {
    this.darkMode = !this.darkMode;

    document.body.classList.toggle('dark-theme', this.darkMode);
    localStorage.setItem('darkMode', this.darkMode ? 'true' : 'false');
  }

  private loadPreference(): void {
    const saved = localStorage.getItem('darkMode');
    this.darkMode = saved === 'true';

    document.body.classList.toggle('dark-theme', this.darkMode);
  }
}
