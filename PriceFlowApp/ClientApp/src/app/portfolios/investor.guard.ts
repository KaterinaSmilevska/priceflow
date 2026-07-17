import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { LoginService } from "../auth/login/login.service";
import { Observable } from "rxjs";
import { map, switchMap } from 'rxjs/operators'

@Injectable({
  providedIn: 'root'
})
export class InvestorGuard implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.loginService.authReady().pipe(
      switchMap(() => this.loginService.isLoggedIn()),
      switchMap(loggedIn => {
        if (!loggedIn) {
          this.router.navigate(['/login']);
          return [false];
        }
        return this.loginService.isInvestor();
      }),
      map(isInvestor => {
        if (!isInvestor) {
          this.router.navigate(['/']);
          return false;
        }
        return true;
      })
    );
  }
}
