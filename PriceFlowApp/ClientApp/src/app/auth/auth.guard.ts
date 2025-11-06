import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, switchMap, take } from 'rxjs/operators';
import { LoginService } from './login/login.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.loginService.isLoggedIn().pipe(
      take(1),
      map(isLoggedIn => {
        if (!isLoggedIn) {
          this.router.navigate(['/login']);
          return false;
        }
        return true;
      })
    );
  }
}

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private loginService: LoginService, private router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.loginService.isLoggedIn().pipe(
      take(1),
      switchMap(isLoggedIn => {
        if (!isLoggedIn) {
          this.router.navigate(['/login']);
          return of(false);
        }
        return this.loginService.getUserRoles().pipe(
          take(1),
          map(roles => {
            const requiredRoles = route.data['roles'] as string[] || ['Администратор'];
            const hasRequiredRole = requiredRoles.some(role => roles.includes(role));
            if (!hasRequiredRole) {
              this.router.navigate(['/login']);
              return false;
            }
            return true;
          })
        );
      })
    );
  }
}
