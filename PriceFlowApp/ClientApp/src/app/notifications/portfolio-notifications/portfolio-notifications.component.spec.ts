import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortfolioNotificationsComponent } from './portfolio-notifications.component';

describe('PortfolioNotificationsComponent', () => {
  let component: PortfolioNotificationsComponent;
  let fixture: ComponentFixture<PortfolioNotificationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PortfolioNotificationsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PortfolioNotificationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
