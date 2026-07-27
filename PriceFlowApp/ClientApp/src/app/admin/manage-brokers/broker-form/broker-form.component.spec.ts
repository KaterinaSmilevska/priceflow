import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BrokerFormComponent } from './broker-form.component';

describe('BrokerFormComponent', () => {
  let component: BrokerFormComponent;
  let fixture: ComponentFixture<BrokerFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BrokerFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BrokerFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
