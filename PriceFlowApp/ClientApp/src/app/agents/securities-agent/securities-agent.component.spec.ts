import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SecuritiesAgentComponent } from './securities-agent.component';

describe('SecuritiesAgentComponent', () => {
  let component: SecuritiesAgentComponent;
  let fixture: ComponentFixture<SecuritiesAgentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SecuritiesAgentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SecuritiesAgentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
