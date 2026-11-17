import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './welcome.html',
  styleUrl: './welcome.scss',
})
export class Welcome {
  private authService = inject(AuthService);
  private router = inject(Router);

  user = this.authService.getCurrentUser();

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
