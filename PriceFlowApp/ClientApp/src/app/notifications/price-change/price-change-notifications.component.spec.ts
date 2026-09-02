import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceChangeNotificationsComponent } from './price-change-notifications.component';

describe('PriceChangeNotificationsComponent', () => {
  let component: PriceChangeNotificationsComponent;
  let fixture: ComponentFixture<PriceChangeNotificationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceChangeNotificationsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PriceChangeNotificationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
