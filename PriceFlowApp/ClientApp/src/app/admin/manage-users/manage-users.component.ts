import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EditUserComponent } from './edit-user/edit-user.component';
import { AdminService } from '../admin.service';
import { User } from './User';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { DbValueTranslatePipe } from '../../shared/db-value-translate.pipe';

@Component({
  selector: 'app-manage-users',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, EditUserComponent, TranslateModule, DbValueTranslatePipe],
  templateUrl: './manage-users.component.html',
  styleUrl: './manage-users.component.css',
})
export class ManageUsersComponent implements OnInit {
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
        this.errorMessage = 'LOADING_DATA_ERROR';
      }
    });
  }

  private loadCurrentUserId(): void {
    this.adminService.getCurrentUserStatus().subscribe({
      next: (status) => {
        if (status.isLoggedIn && status.userId) {
          this.currentUserId = Number(status.userId);
        }
      },
      error: (err) => {
        this.errorMessage = 'USERS.USER_STATUS_ERROR';
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
        this.errorMessage = 'USERS.DELETE_ERROR';
        this.closeDeleteModal();
      }
    });
  }

}
