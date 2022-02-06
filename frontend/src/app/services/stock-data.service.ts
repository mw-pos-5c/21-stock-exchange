import {Injectable} from '@angular/core';
import {SignalrService} from "./signalr.service";
import {Subject} from "rxjs";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class StockDataService {

  constructor(private signal: SignalrService, private http: HttpClient) {

    this.signal.ready(() => {


      this.http.get<any>("http://localhost:5174/api/stock/getshares").subscribe(value => {
        this.shares = value;
        console.log(value);
      });

      this.signal.client.on("NewStockData", data => {

        if (this.priceHistory.labels === undefined) this.priceHistory.labels = [];
        if (this.priceHistory.stock === undefined) this.priceHistory.stock = {};

        let x = 0
        for (let stock of data) {
          if (x >= 5) break; x++;
          if (this.priceHistory.stock[stock.name] === undefined) this.priceHistory.stock[stock.name] = [];
          this.priceHistory.stock[stock.name].push(stock.val);
        }
        this.priceHistory.labels.push(new Date().toUTCString());
        this.priceHistoryUpdated.next(this.priceHistory);

      });

      this.signal.client.on("ClientCountChanged", count => {
        this.clientCount = count;
      })

      this.signal.client.on("TransactionReceived", trans => {
        this.log.push("new transaction");
      })

    });

  }

  public priceHistoryUpdated = new Subject<any>();

  public clientCount: number = 0;
  public shares: any[] = [];
  public priceHistory: any = {};

  public log: string[] = [];
}
