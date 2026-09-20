import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(public authService: AuthService, private router: Router) { }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  requestRecruiter(): void {
    this.authService.requestRecruiterRole().subscribe({
      next: () => alert('Zahtjev poslan. Admin će ga uskoro pregledati.'),
      error: (err) => alert(err.error?.message ?? 'Zahtjev nije uspio.')
    });
  }

  protected readonly title = signal('recruitment-pipeline-client');
}
