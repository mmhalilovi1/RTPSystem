import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Application, ApplicationService } from '../../core/services/application.service';

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './my-applications.component.html',
  styleUrl: './my-applications.component.css'
})
export class MyApplicationsComponent implements OnInit {
  applications = signal<Application[]>([]);
  isLoading = signal(false);

  constructor(private applicationService: ApplicationService, private router: Router) { }

  ngOnInit(): void {
    this.isLoading.set(true);
    this.applicationService.getMyApplications().subscribe({
      next: (apps) => {
        this.applications.set(apps);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  viewApplication(id: string): void {
    this.router.navigate(['/applications', id]);
  }
}
