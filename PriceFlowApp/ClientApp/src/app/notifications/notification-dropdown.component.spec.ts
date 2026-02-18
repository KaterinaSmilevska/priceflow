import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotificationsDropDownComponent } from './notifications-dropdown.component';

describe('NotificationsDropDownComponent', () => {
  let component: NotificationsDropDownComponent;
  let fixture: ComponentFixture<NotificationsDropDownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotificationsDropDownComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(NotificationsDropDownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
