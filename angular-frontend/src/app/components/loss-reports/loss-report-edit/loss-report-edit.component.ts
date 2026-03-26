import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LossReportService } from '../../../services/loss-report.service';

@Component({
  selector: 'app-loss-report-edit',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './loss-report-edit.component.html',
  styleUrls: ['./loss-report-edit.component.css']
})
export class LossReportEditComponent implements OnInit {
  form: FormGroup;
  isSubmitting = false;
  isLoading = true;
  reportId = 0;

  constructor(
    private fb: FormBuilder,
    private lossReportService: LossReportService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      policyNumber: ['', [Validators.required, Validators.maxLength(50)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      amount: [null, [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.reportId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadReport();
  }

  loadReport(): void {
    this.lossReportService.getById(this.reportId).subscribe({
      next: (report) => {
        this.form.patchValue({
          policyNumber: report.policyNumber,
          description: report.description,
          amount: report.amount
        });
        this.isLoading = false;
      },
      error: () => {
        this.snackBar.open('Failed to load loss report.', 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.router.navigate(['/loss-reports']);
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.lossReportService.update(this.reportId, this.form.value).subscribe({
      next: () => {
        this.snackBar.open('Loss report updated successfully!', 'Close', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/loss-reports']);
      },
      error: () => {
        this.snackBar.open('Failed to update loss report. Please try again.', 'Close', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.isSubmitting = false;
      }
    });
  }
}
