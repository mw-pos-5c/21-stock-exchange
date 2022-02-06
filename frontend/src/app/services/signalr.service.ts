import { Injectable } from '@angular/core';
import {HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private client: HubConnection;

  public on: (methodName: string, newMethod: (...args: any[]) => void) => void;
  public send: (methodName: string, ...args: any[]) => Promise<void>;
  public invoke: <T = any>(methodName: string, ...args: any[]) => Promise<T>;

  constructor() {
    this.client = new HubConnectionBuilder()
      .withUrl(`http://localhost:5174/ws`)
      .configureLogging(LogLevel.Debug)
      .withAutomaticReconnect()
      .build();

    this.on = this.client.on;
    this.send = this.client.send;
    this.invoke = this.client.invoke;

    this.client.start();
  }


}
