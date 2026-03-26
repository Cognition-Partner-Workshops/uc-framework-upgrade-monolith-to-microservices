import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { LossReport, CreateLossReport, UpdateLossReport } from '../models/loss-report.model';

@Injectable({
  providedIn: 'root'
})
export class LossReportService {
  private apiUrl = `${environment.apiUrl}/api/lossreports`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<LossReport[]> {
    return this.http.get<LossReport[]>(this.apiUrl);
  }

  getById(id: number): Observable<LossReport> {
    return this.http.get<LossReport>(`${this.apiUrl}/${id}`);
  }

  create(report: CreateLossReport): Observable<LossReport> {
    return this.http.post<LossReport>(this.apiUrl, report);
  }

  update(id: number, report: UpdateLossReport): Observable<LossReport> {
    return this.http.put<LossReport>(`${this.apiUrl}/${id}`, report);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
