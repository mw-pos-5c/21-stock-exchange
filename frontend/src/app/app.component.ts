import {Component, OnInit} from '@angular/core';
import {SignalrService} from "./services/signalr.service";
import {StockDataService} from "./services/stock-data.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {


  public historyData: any = {};

  constructor(public data: StockDataService) {

    data.priceHistoryUpdated.subscribe((data) => {
      this.historyData = {...data};
    })

  }

  ngOnInit(): void {
  }
}
