import { Component, OnInit } from '@angular/core';
import { AdminService, User } from './admin.service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  users: User[] = [];
  errorMessage: string | null = null;
  showDeleteModal = false;
  userToDelete: User | null = null;
  currentUserId: number| null = null;

  constructor(private adminService: AdminService, private router: Router) { }

  ngOnInit(): void {
    this.loadUsers();
    this.loadCurrentUserId();
  }

  loadUsers(): void {
    this.adminService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
      },
      error: (err) => {
        console.error('Failed to load users:', err);
        this.errorMessage = 'Failed to load users.';
      }
    });
  }

  private loadCurrentUserId(): void {
    this.adminService.getCurrentUserStatus().subscribe({
      next: (status) => {
        if (status.isLoggedIn && status.userId) {
          this.currentUserId = status.userId;
        }
      },
      error: (err) => {
        console.error('Failed to get user status:', err);
      }
    });
  }

  editUser(userId: number): void {
    this.router.navigate([`/admin/users/${userId}`]);
  }

  openDeleteModal(user: User): void {
    this.userToDelete = user;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.userToDelete = null;
  }

  confirmDelete(): void {
    if (!this.userToDelete) return;

    this.adminService.deleteUser(this.userToDelete.id).subscribe({
      next: () => {
        this.loadUsers();
        this.closeDeleteModal();
      },
      error: (err) => {
        console.error('Failed to delete user:', err);
        this.errorMessage = 'Failed to delete user.';
        this.closeDeleteModal();
      }
    });
  }
}
