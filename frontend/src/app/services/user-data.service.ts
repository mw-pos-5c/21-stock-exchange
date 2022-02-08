import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {SignalrService} from "./signalr.service";

@Injectable({
  providedIn: 'root'
})
export class UserDataService {

  constructor(private http: HttpClient, private signal: SignalrService) {
  }

  getUserShares(name: string) {
    return this.http.get<any>("http://localhost:5174/api/stock/GetUserDepots?name=" + name)
  }

  getUserCash(name: string) {
    return this.http.get<any>("http://localhost:5174/api/stock/GetUserCash?name=" + name);
  }


  newTransaction(name: string, share: string, buy: boolean, value: number) {
    console.log("SENT");
    this.signal.client.send("AddTransaction", name, share, value, buy);
  }
}
