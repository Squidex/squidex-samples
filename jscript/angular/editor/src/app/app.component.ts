import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

declare var SquidexFormField: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  form = new FormControl();

  constructor() {
    const field = new SquidexFormField();

    field.onDisabled((disabled: boolean) => {
      if (disabled) {
        this.form.disable();
      } else {
        this.form.enable();
      }
    });

    field.onValueChanged((value: any) => {
      this.form.setValue(value);
    });

    this.form.valueChanges.subscribe(value => {
      field.valueChanged(value);
    })
  }
}
