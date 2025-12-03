import { Component, OnInit } from '@angular/core';
import { AdminService, User } from './admin.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EditUserComponent } from './edit-user.component';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, EditUserComponent],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})

export class AdminComponent implements OnInit {
  users: User[] = [];
  errorMessage: string | null = null;
  showDeleteModal = false;
  userToDelete: User | null = null;
  currentUserId: number | null = null;
  showEditModal = false;
  userToEdit: User | null = null;

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
    const user = this.users.find(u => u.id === userId);
    if (user)
      this.openEditModal(user);
  }

  onEditModalClose(updated: User | null) {
    this.showEditModal = false;

    if (updated) {
      const index = this.users.findIndex(u => u.id === updated.id);
      if (index > -1)
        this.users[index] = updated;
    }
  }

  openEditModal(user: User): void {
    this.userToEdit = user;
    this.showEditModal = true;
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
