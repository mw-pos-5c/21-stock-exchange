import {Component, OnInit} from '@angular/core';
import {SignalrService} from "./services/signalr.service";
import {StockDataService} from "./services/stock-data.service";
import {UserDataService} from "./services/user-data.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {


  public historyData: any = {};
  public selectedUserMoney: number = 0;
  public selectedUserShares: any[] = [];
  public selectedUserName: string = '';
  public selectedShare: string = '';

  constructor(public data: StockDataService, public signal: SignalrService, private userData: UserDataService) {

    data.priceHistoryUpdated.subscribe((data) => {
      this.historyData = {...data};
    })

  }

  ngOnInit(): void {
    this.data.newTransaction.subscribe(() => this.loadUserData());
  }

  connect() {
    this.signal.connect();
  }

  userNameChanged(e: Event) {
    // @ts-ignore
    this.selectedUserName = e.target?.value

    this.loadUserData();

  }

  loadUserData() {

    if (this.selectedUserName.length < 1) return;

    this.userData.getUserShares(this.selectedUserName).subscribe(value => {
      this.selectedUserShares = value;
    })

    this.userData.getUserCash(this.selectedUserName).subscribe(value => {
      this.selectedUserMoney = value;
    })
  }

  addTransaction(value: number, isBuy: boolean) {
    console.log(this.selectedShare);
    if (this.selectedShare.length < 1) return;
    this.userData.newTransaction(this.selectedUserName, this.selectedShare, isBuy, value);
  }

}
