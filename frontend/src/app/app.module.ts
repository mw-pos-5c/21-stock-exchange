import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { StockChartComponent } from './components/stock-chart/stock-chart.component';
import {NgChartsModule} from "ng2-charts";
import { LogComponent } from './components/log/log.component';
import {HttpClientModule} from "@angular/common/http";

@NgModule({
  declarations: [
    AppComponent,
    StockChartComponent,
    LogComponent
  ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        NgChartsModule,
        HttpClientModule
    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
