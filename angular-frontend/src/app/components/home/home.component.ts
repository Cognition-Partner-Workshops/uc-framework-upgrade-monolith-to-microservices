import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  helloMessage: string = '';
  loading: boolean = false;
  error: string = '';
  endpoints: { method: string; path: string; description: string }[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.fetchHelloWorld();
    this.fetchEndpoints();
  }

  fetchHelloWorld(): void {
    this.loading = true;
    this.error = '';
    this.apiService.getHelloWorld().subscribe({
      next: (response) => {
        this.helloMessage = response;
        this.loading = false;
      },
      error: (err) => {
        this.error = `Failed to fetch from API: ${err.message}`;
        this.loading = false;
      }
    });
  }

  fetchEndpoints(): void {
    this.apiService.getSwaggerInfo().subscribe({
      next: (swagger) => {
        this.endpoints = [];
        for (const path in swagger.paths) {
          for (const method in swagger.paths[path]) {
            const operation = swagger.paths[path][method];
            this.endpoints.push({
              method: method.toUpperCase(),
              path: path,
              description: operation.summary || operation.operationId || ''
            });
          }
        }
      },
      error: () => {
        // Swagger info is optional; don't show error
      }
    });
  }
}
