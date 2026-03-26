import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import { LossReportService } from '../../../services/loss-report.service';
import { LossReport } from '../../../models/loss-report.model';

@Component({
  selector: 'app-loss-report-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatCardModule
  ],
  templateUrl: './loss-report-list.component.html',
  styleUrls: ['./loss-report-list.component.css']
})
export class LossReportListComponent implements OnInit {
  reports: LossReport[] = [];
  displayedColumns: string[] = ['id', 'policyNumber', 'description', 'amount', 'createdDate', 'actions'];
  isLoading = true;

  constructor(
    private lossReportService: LossReportService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadReports();
  }

  loadReports(): void {
    this.isLoading = true;
    this.lossReportService.getAll().subscribe({
      next: (data) => {
        this.reports = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.snackBar.open('Failed to load loss reports. Is the backend running?', 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.isLoading = false;
      }
    });
  }

  deleteReport(id: number): void {
    if (confirm('Are you sure you want to delete this loss report?')) {
      this.lossReportService.delete(id).subscribe({
        next: () => {
          this.snackBar.open('Loss report deleted successfully!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.loadReports();
        },
        error: (err) => {
          this.snackBar.open('Failed to delete loss report.', 'Close', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        }
      });
    }
  }

  getTotalAmount(): number {
    return this.reports.reduce((sum, r) => sum + r.amount, 0);
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(amount);
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
