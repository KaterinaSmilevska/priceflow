import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Broker } from './Broker';
import { EditBrokerComponent } from './edit-broker/edit-broker.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-manage-brokers',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, EditBrokerComponent],
  templateUrl: './manage-brokers.component.html',
  styleUrl: './manage-brokers.component.css',
})
export class ManageBrokersComponent implements OnInit {
  brokers: Broker[] = [];
  errorMessage: string | null = null;

  broker: Broker = { id: 0, company: '', commissionPercent: 0 }

  showEditModal = false;
  brokerToEdit: Broker | null = null;
  showDeleteModal = false;
  brokerToDelete: Broker | null = null;


  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.loadBrokers();
  }

  loadBrokers(): void {
    this.adminService.getBrokers().subscribe({
      next: (data) => this.brokers = data,
      error: () => this.errorMessage = "Failed to load brokers."
    });
  }

  openAddModal(): void {
    this.brokerToEdit = { ...this.broker };
    this.showEditModal = true;
  }

  openEditModal(broker: Broker): void {
    this.brokerToEdit = { ...broker };
    this.showEditModal = true;
  }

  onEditModalClose(updated: Broker | null): void {
    this.showEditModal = false;
    if (updated) {
      const index = this.brokers.findIndex(b => b.id === updated.id);
      if (index > -1) this.brokers[index] = updated;
      else this.brokers.push(updated);
    }

    this.broker = { id: 0, company: '', commissionPercent: 0 };
  }

  openDeleteModal(broker: Broker): void {
    this.brokerToDelete = broker;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.brokerToDelete = null;
  }

  confirmDelete(): void {
    if (!this.brokerToDelete) return;

    this.adminService.deleteBroker(this.brokerToDelete.id).subscribe({
      next: () => {
        this.loadBrokers();
        this.closeDeleteModal();
      },
      error: () => {
        this.errorMessage = "Failed to delete broker.";
        this.closeDeleteModal();
      }
    });
  }
}
