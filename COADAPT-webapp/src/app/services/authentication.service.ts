import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { RepositoryService } from './repository.service';

import { LoginUser } from '../models';

import { APIv1 } from '../constants';

@Injectable({
  providedIn: 'root',
})
export class AuthenticationService {
  private currentUserSubject: BehaviorSubject<LoginUser>;
  private refreshTokenTimeout;

  public currentUser: Observable<LoginUser>;

  public get currentUserValue(): LoginUser {
    return this.currentUserSubject.value;
  }

  constructor(private repository: RepositoryService) {
    this.currentUserSubject = new BehaviorSubject<LoginUser>(
      JSON.parse(localStorage.getItem('currentUser'))
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  private storeUser(user: any): void {
    this.currentUserSubject.next(user);
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  private startRefreshTokenTimer() {
    const timeout = 5000;

    this.refreshTokenTimeout = setTimeout(
      () => this.refreshToken().subscribe(),
      timeout
    );
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }

  login(userName: string, password: string) {
    return this.repository
      .postData(APIv1.accountLogin, { userName, password }).pipe(
        map((user) => {
          this.storeUser(user);
          // this.startRefreshTokenTimer();
          return user;
        })
      );
  }

  logout() {
    // probably a revoke token call.
    this.stopRefreshTokenTimer();
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  refreshToken() {
    if (!this.currentUserValue) return;
    const refreshToken = this.currentUserValue.refreshToken;

    return this.repository.postData(APIv1.accountRefreshToken, { refreshToken }).pipe(
      map((user) => {
        this.storeUser(user.value);
        this.startRefreshTokenTimer();

        return user;
      })
    );
  }
}
