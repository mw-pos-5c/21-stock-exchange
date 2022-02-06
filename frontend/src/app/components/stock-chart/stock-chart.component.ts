import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ChartConfiguration, ChartType} from 'chart.js';
import {SignalrService} from "../../services/signalr.service";
import {BaseChartDirective} from "ng2-charts";

@Component({
  selector: 'app-stock-chart',
  templateUrl: './stock-chart.component.html',
  styleUrls: ['./stock-chart.component.scss']
})
export class StockChartComponent implements OnInit {



  @Input() set dataSource(data: any) {
    console.log("update", data);
    const datasets = [];
    for (let label in data.stock) {
      datasets.push({
        data: data.stock[label],
        label: label,

        fill: 'origin',
      });
    }

    this.lineChartData.datasets = datasets;
    this.lineChartData.labels = data.labels;
    this.chart?.update();
  }

  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  constructor(private signal: SignalrService) {
  }

  ngOnInit(): void {

    this.signal.ready(() => {



    });


  }

  lineChartData: ChartConfiguration['data'] = {
    datasets: [],
    labels: []
  };

  lineChartOptions: ChartConfiguration['options'] = {
    animation: false,
    elements: {
      line: {
        tension: 0.5
      }
    },
    scales: {
      x: {},
      'y-axis-0':
        {
          position: 'left',
        }
    }
  };

  public lineChartType: ChartType = 'line';

}
