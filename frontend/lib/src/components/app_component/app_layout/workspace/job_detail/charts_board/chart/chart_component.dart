import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart/chart_strategy.dart';
import 'package:sw_project/src/interop/toastr.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/services/base/exceptions.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'chart',
  directives: [
    formDirectives,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialIconComponent,
    MaterialCheckboxComponent,
    MaterialProgressComponent,
    materialInputDirectives,

    MaterialMultilineInputComponent,
    materialNumberInputDirectives,
    MaterialPaperTooltipComponent,
    MaterialTooltipTargetDirective,

    NgIf,
    NgFor
  ],
  templateUrl: 'chart_component.html',
  styleUrls: ['chart_component.css'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    materialProviders,
  ],
)
class ChartComponent implements AfterChanges {

  final SocnetoService _socnetoService;

  @Input() ChartDefinition chartDefinition;
  @Input() String jobId;
  @Input() String chartId;

  ChartStrategy _chartStrategy;
  List<List<List<dynamic>>> chartData = [];
  bool loadingData = true;
  
  ChartComponent(this._socnetoService);

  @override
  void ngAfterChanges() async {
    this._setChartStrategy();
    this._refreshChart();
  }

  String get chartTitle => this.chartDefinition?.title != null && this.chartDefinition.title.isNotEmpty? this.chartDefinition.title : "Chart";
  
  void _setChartStrategy() {
    switch (this.chartDefinition.chartType) {
      case ChartType.Line:
        this._chartStrategy = LineChartStrategy(); break;
      case ChartType.Pie:
        this._chartStrategy = PieChartStrategy(); break;
      case ChartType.Scatter:
        this._chartStrategy = ScatterChartStrategy(); break;
    }
  }

  void _refreshChart() async {
    try {
      this.chartData = await this._socnetoService.getChartData(this.jobId, this.chartDefinition);
    } on HttpException {
      Toastr.error("Analysis", "Could not fetch analyses for chart");
      return;
    } finally {
      this.loadingData = false;
    }

    if (this.chartData.isNotEmpty) {
      this._chartStrategy.setData(this.chartDefinition, this.chartData);
      // TODO: HACK HERE ! The charts needs to be created after this element was already created
      Timer(Duration(milliseconds: 500), this._redrawChart);
    }
  }

  void _redrawChart() {
    var domSelector = "#${this.chartId} .chart-wrapper";
    this._chartStrategy.redrawChart(domSelector);
  }

}
