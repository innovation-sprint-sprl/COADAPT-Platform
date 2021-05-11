import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class RepositoryService {
  constructor(private http: HttpClient) {}

  private createCompleteRoute(route: string) {
    return `${environment.apiURL}/${route}`;
  }

  public getData(route: string) {
    return this.http.get<any>(this.createCompleteRoute(route));
  }

  public postData(route: string, body) {
    return this.http.post<any>(this.createCompleteRoute(route), body, this.generateHeaders());
  }

  public update(route: string, body) {
    return this.http.put<any>(this.createCompleteRoute(route), body, this.generateHeaders());
  }

  public delete(route: string) {
    return this.http.delete(this.createCompleteRoute(route));
  }

  private generateHeaders() {
    return { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };
  }
}
