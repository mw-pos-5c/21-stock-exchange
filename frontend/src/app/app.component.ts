import { Component } from '@angular/core';
import {SignalrService} from "./services/signalr.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  constructor(private signal: SignalrService) {
  }
}
