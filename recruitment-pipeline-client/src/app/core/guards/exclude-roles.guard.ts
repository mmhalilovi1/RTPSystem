import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export function excludeRolesGuard(blockedRoles: string[]): CanActivateFn {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (!authService.isLoggedIn()) {
      router.navigate(['/login']);
      return false;
    }

    const role = authService.role();
    if (role && blockedRoles.includes(role)) {
      router.navigate(['/positions']);
      return false;
    }

    return true;
  };
}
