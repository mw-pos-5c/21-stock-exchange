import {Injectable} from '@angular/core';
import {HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {Subject} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  client: HubConnection;

  private connectionReady = new Subject<void>();
  public connected = false;

  constructor() {
    this.client = new HubConnectionBuilder()
      .withUrl(`http://localhost:5174/ws`)
      .configureLogging(LogLevel.Debug)
      .withAutomaticReconnect()
      .build();
  }

  ready(ready: () => void) {
    if (this.connected) ready();

    const sub = this.connectionReady.subscribe({
      complete: () => {
        ready();
        sub.unsubscribe();
      }
    });

  }

  connect() {
    this.client.start().then(_ => {
      this.connected = true;
      this.connectionReady.complete();
    });
  }
}
