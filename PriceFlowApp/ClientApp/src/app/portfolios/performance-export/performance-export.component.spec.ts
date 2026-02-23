import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PerformanceExportComponent } from './performance-export.component';

describe('PerformanceExportComponent', () => {
  let component: PerformanceExportComponent;
  let fixture: ComponentFixture<PerformanceExportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PerformanceExportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PerformanceExportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
