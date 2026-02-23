import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortfolioIncomeComponent } from './portfolio-income.component';

describe('PortfolioIncomeComponent', () => {
  let component: PortfolioIncomeComponent;
  let fixture: ComponentFixture<PortfolioIncomeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PortfolioIncomeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PortfolioIncomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
