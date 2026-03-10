import { Component, OnInit } from '@angular/core';
import { SecuritiesService, Security } from './securities.service';
import { LoginService } from '../auth/login/login.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AddSecurityComponent } from './add-security/add-security.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-securities',
  standalone: true,
  imports: [CommonModule, FormsModule, AddSecurityComponent, TranslateModule],
  templateUrl: './securities.component.html',
  styleUrls: ['./securities.component.css']
})
export class SecuritiesComponent implements OnInit {
  securities: Security[] = [];
  loading: boolean = true;
  errorMessage: string | null = null;
  showDeleteModal = false;
  securityToDelete: Security | null = null;
  showEditModal = false;
  securityToEdit: Security | undefined = undefined;

  isAdmin = false;

  searchTerm: string = '';
  loadingSearch = false;

  constructor(private securitiesService: SecuritiesService, public loginService: LoginService) { }

  ngOnInit(): void {
    this.loadSecurities();
    this.loginService.getUserRoles().subscribe(roles => {
      this.isAdmin = roles.includes('Администратор');
    })
  }
 

  loadSecurities(): void {
    this.securitiesService.getAll().subscribe({
      next: (securities) => {
        this.securities = securities;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = 'LOADING_DATA_ERROR';
        this.loading = false;
      }
    });
  }

  openDeleteModal(security: Security): void {
    this.securityToDelete = security;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.securityToDelete = null;
  }

  confirmDelete(): void {
    if (!this.securityToDelete) return;

    this.securitiesService.deleteSecurity(this.securityToDelete.id).subscribe({
      next: () => {
        this.securities = this.securities.filter(s => s.id !== this.securityToDelete?.id);
        this.closeDeleteModal();
      },
      error: (err) => {
        this.errorMessage = 'SECURITIES.DELETE_ERROR';
        this.closeDeleteModal();
      }
    });
  }

  openAddModal() {
    this.securityToEdit = undefined;
    this.showEditModal = true;
  }

  openEditModal(security: Security) {
    this.securityToEdit = security;
    this.showEditModal = true;
  }

  onModalClose(updatedSecurity: Security | null) {
    this.showEditModal = false;
    if (!updatedSecurity) return;

    if (this.securityToEdit) {
      const index = this.securities.findIndex(s => s.id === updatedSecurity.id);
      if (index > -1) this.securities[index] = updatedSecurity;
    } else {
      this.securities.push(updatedSecurity);
    }
  }

  onSearch(): void {
    const searchTerm = this.searchTerm.trim();
    if (!searchTerm) {
      this.loadSecurities();
      return;
    }
    this.loadingSearch = true;

    this.securitiesService.searchByCode(searchTerm)
      .subscribe({
        next: (res) => {
          this.securities = res;
          this.loadingSearch = false;
        },
        error: () => {
          this.securities = [];
          this.loadingSearch = false;
        }
     });
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.loadSecurities();
  }
}
