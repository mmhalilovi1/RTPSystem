import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdminService, PendingRecruiter } from '../../core/services/admin.service';

@Component({
  selector: 'app-admin-recruiters',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-recruiters.component.html',
  styleUrl: './admin-recruiters.component.css'
})
export class AdminRecruitersComponent implements OnInit {
  pending = signal<PendingRecruiter[]>([]);
  isLoading = signal(false);

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.load();
  }

  approve(userId: string): void {
    this.adminService.approveRecruiter(userId).subscribe({ next: () => this.load() });
  }

  reject(userId: string): void {
    this.adminService.rejectRecruiter(userId).subscribe({ next: () => this.load() });
  }

  private load(): void {
    this.isLoading.set(true);
    this.adminService.getPendingRecruiters().subscribe({
      next: (list) => {
        this.pending.set(list);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
}
