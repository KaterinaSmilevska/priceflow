import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageBrokersComponent } from './manage-brokers.component';

describe('ManageBrokersComponent', () => {
  let component: ManageBrokersComponent;
  let fixture: ComponentFixture<ManageBrokersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageBrokersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageBrokersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
