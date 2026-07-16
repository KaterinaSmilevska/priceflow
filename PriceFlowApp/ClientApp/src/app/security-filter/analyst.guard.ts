import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { LoginService } from "../auth/login/login.service";
import { Observable } from "rxjs";
import { map, switchMap, tap } from 'rxjs/operators'

@Injectable({ providedIn: 'root' })
export class AnalystGuard implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.loginService.isLoggedIn().pipe(
      map(loggedIn => {
        if (!loggedIn) {
          this.router.navigate(['/login']);
          return false;
        }
        return true;
      }),
      switchMap(() => this.loginService.isAnalyst()),
      tap(isAnalyst => {
        if (!isAnalyst) {
          this.router.navigate(['/']);
        }
      })
    );
  }
}
