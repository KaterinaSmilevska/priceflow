import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortfolioSecurityAllocationComponent } from './portfolio-security-allocation.component';

describe('PortfolioSecurityAllocationComponent', () => {
  let component: PortfolioSecurityAllocationComponent;
  let fixture: ComponentFixture<PortfolioSecurityAllocationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PortfolioSecurityAllocationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PortfolioSecurityAllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
