import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PriceChangeNotificationsDropDownComponent } from './price-change-notifications-dropdown.component';

describe('PriceChangeNotificationsDropDownComponent', () => {
  let component: PriceChangeNotificationsDropDownComponent;
  let fixture: ComponentFixture<PriceChangeNotificationsDropDownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PriceChangeNotificationsDropDownComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(PriceChangeNotificationsDropDownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
