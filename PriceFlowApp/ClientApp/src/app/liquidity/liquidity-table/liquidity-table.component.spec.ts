import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LiquidityTableComponent } from './liquidity-table.component';

describe('LiquidityTableComponent', () => {
  let component: LiquidityTableComponent;
  let fixture: ComponentFixture<LiquidityTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LiquidityTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LiquidityTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
