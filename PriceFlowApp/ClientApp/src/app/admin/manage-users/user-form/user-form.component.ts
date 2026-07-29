import { Component, Input, Output, EventEmitter, OnInit, SimpleChanges, OnChanges } from '@angular/core';
import { AdminService } from '../../admin.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { User } from '../User';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})

export class UserFormComponent implements OnInit, OnChanges {
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

    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const updatedUser: User = {
      ...this.userToEdit,
      ...this.editForm.value
    }

    this.adminService.checkUsername(updatedUser.username).subscribe({
      next: (res) => {
        if (res.exists && updatedUser.username !== this.originalUsername) {
          this.errorMessage = 'USERS.USERNAME_TAKEN';
          return;
        }

        this.adminService.updateUser(updatedUser).subscribe({
          next: () => {
            this.successMessage = 'USERS.UPDATE_SUCCESS';
            setTimeout(() => this.close.emit(updatedUser), 800);
          },
          error: () => {
            this.errorMessage = 'USERS.UPDATE_ERROR';
          }
        });
      },
      error: () => {
        this.errorMessage = 'USERS.USERNAME_CHECK_ERROR';
      }
    });
  }

  cancel(): void {
    this.close.emit(null);
  }
}
