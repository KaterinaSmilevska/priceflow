import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ThresholdFormComponent } from './threshold-form.component';

describe('ThresholdFormComponent', () => {
  let component: ThresholdFormComponent;
  let fixture: ComponentFixture<ThresholdFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ThresholdFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ThresholdFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
