import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getHelloWorld(): Observable<string> {
    return this.http.get(`${this.baseUrl}/api/HelloWorld`, { responseType: 'text' });
  }

  getSwaggerInfo(): Observable<any> {
    return this.http.get(`${this.baseUrl}/swagger/v1/swagger.json`);
  }
}
