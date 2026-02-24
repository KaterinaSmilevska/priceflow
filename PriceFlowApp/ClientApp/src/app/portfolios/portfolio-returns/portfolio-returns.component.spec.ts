import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortfolioReturnsComponent } from './portfolio-returns.component';

describe('PortfolioReturnsComponent', () => {
  let component: PortfolioReturnsComponent;
  let fixture: ComponentFixture<PortfolioReturnsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PortfolioReturnsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PortfolioReturnsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
