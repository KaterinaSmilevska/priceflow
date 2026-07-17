import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { LoginService } from "../auth/login/login.service";
import { Observable } from "rxjs";
import { map, switchMap } from 'rxjs/operators'

@Injectable({
  providedIn: 'root'
})
export class AnalystGuard implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.loginService.authReady().pipe(
      switchMap(() => this.loginService.isLoggedIn()),
      switchMap(loggedIn => {
        if (!loggedIn) {
          this.router.navigate(['/login']);
          return [false];
        }
        return this.loginService.isAnalyst();
      }),
      map(isAnalyst => {
        if (!isAnalyst) {
          this.router.navigate(['/']);
          return false;
        }
        return true;
      })
    );
  }
}
