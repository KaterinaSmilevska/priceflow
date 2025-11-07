import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet } from '@angular/router';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterOutlet, NavMenuComponent],
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
}
