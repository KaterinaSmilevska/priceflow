import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AdminService, User } from './admin.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-edit-user',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.css', '../../styles.css']
})

export class EditUserComponent implements OnInit {
  user: User = { id: 0, name: '', username: '', email: '', roles: [] };
  originalUsername: string | null = null;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const userId = +this.route.snapshot.paramMap.get('id')!;
    this.adminService.getUser(userId).subscribe({
      next: (user) => {
        this.user = { ...user };
        this.originalUsername = user.username;
      },
      error: (err) => {
        console.error('Failed to load user:', err);
        this.errorMessage = 'Failed to load user details.';
      }
    });
  }

  saveUser(): void {
    this.errorMessage = null;
    this.successMessage = null;

    this.adminService.checkUsername(this.user.username).subscribe({
      next: (res) => {
        if (res.exists && this.user.username !== this.originalUsername) {
          this.errorMessage = 'Username is already taken.';
          return;
        }

        this.adminService.updateUser(this.user).subscribe({
          next: () => {
            this.successMessage = 'User updated successfully!';
            setTimeout(() => this.router.navigate(['/admin/users']), 2000);
          },
          error: (err) => {
            console.error('Failed to update user:', err);
            this.errorMessage = 'Failed to update user.';
          }
        });
      },
      error: (err) => {
        console.error('Failed to check username:', err);
        this.errorMessage = 'Error checking username availability.';
      }
    });
  }

}
