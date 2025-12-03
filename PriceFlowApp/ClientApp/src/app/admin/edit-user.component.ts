import { Component, Input, Output, EventEmitter, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AdminService, User } from './admin.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-edit-user',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.css', '../../styles.css']
})

export class EditUserComponent implements OnInit {
  @Input() userToEdit!: User;
  @Output() close = new EventEmitter<User | null>();

  editForm!: FormGroup;

  originalUsername: string | null = null;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService
  ) { }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['userToEdit'] && this.userToEdit) {
      this.originalUsername = this.userToEdit.username;

      this.editForm = this.fb.group({
        name: [this.userToEdit.name || '', Validators.required],
        username: [this.userToEdit.username || '', Validators.required],
        email: [this.userToEdit.email || '', [Validators.required, Validators.email]]
      });
    }
  }

  saveUser(): void {
    this.errorMessage = null;
    this.successMessage = null;

    if (this.editForm.invalid) return;

    const updatedUser: User = {
      ...this.userToEdit,
      ...this.editForm.value
    }

    this.adminService.checkUsername(updatedUser.username).subscribe({
      next: (res) => {
        if (res.exists && updatedUser.username !== this.originalUsername) {
          this.errorMessage = 'Username is already taken.';
          return;
        }

        this.adminService.updateUser(updatedUser).subscribe({
          next: () => {
            this.successMessage = 'User updated successfully!';
            setTimeout(() => this.close.emit(updatedUser), 1000);
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

  cancel(): void {
    this.close.emit(null);
  }

}
