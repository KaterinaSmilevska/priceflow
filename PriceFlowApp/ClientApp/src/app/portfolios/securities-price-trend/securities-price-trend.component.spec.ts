import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SecuritiesPriceTrendComponent } from './securities-price-trend.component';

describe('SecuritiesPriceTrendComponent', () => {
  let component: SecuritiesPriceTrendComponent;
  let fixture: ComponentFixture<SecuritiesPriceTrendComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SecuritiesPriceTrendComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SecuritiesPriceTrendComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
