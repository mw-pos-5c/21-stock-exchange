import { Component, OnInit } from '@angular/core';
import {StockDataService} from "../../services/stock-data.service";

@Component({
  selector: 'app-log',
  templateUrl: './log.component.html',
  styleUrls: ['./log.component.scss']
})
export class LogComponent implements OnInit {

  constructor(public data: StockDataService) { }

  ngOnInit(): void {
  }

}
