import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_forms/angular_forms.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart/chart_strategy.dart';
import 'package:sw_project/src/components/shared/verification/verification_modal.dart';
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

    VerificationModal,

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

  @ViewChild(VerificationModal) VerificationModal verificationModal;

  final int PAGE_SIZE = 200;
  final SocnetoService _socnetoService;

  final _updateChartsController = StreamController<List<ChartDefinition>>();
  @Output() Stream<List<ChartDefinition>> get updateCharts => _updateChartsController.stream;

  @Input() ChartDefinition chartDefinition;
  @Input() String jobId;
  @Input() String chartId;
  @Input() bool removable = true;

  ChartStrategy _chartStrategy;
  List<List<List<dynamic>>> chartData = [];
  bool loadingData = true;
  int currentPage = 1;
  int totalResults = 0;
  int maxPage = 1;
  bool isPaginated = false;
  
  ChartComponent(this._socnetoService);

  @override
  void ngAfterChanges() async {
    this._setChartStrategy();
    this._refreshChart();
  }

  String get chartTitle => this.chartDefinition?.title != null && this.chartDefinition.title.isNotEmpty? this.chartDefinition.title : "Chart";

  void removeChart() {
    this.verificationModal.show();
  }

  void onRemoveChartVerified() async {
    try {
      var newCharts = await this._socnetoService.removeChartDefinition(jobId, chartDefinition.id);
      Toastr.success("Success", "The chart was successfully removed");
      this._updateChartsController.add(newCharts);
    } on HttpException catch (e) {
      Toastr.httpError(e);
    }
  }

  void onPageBack() {
    this._changePageTo(this.currentPage + 1);
  }

  void onPageForward() {
    this._changePageTo(this.currentPage - 1);
  }

  void onRefresh() {
    this._refreshChart();
  }

  void _changePageTo(int page) {
    if (page < 0 || page > this.maxPage) return;

    this.currentPage = page;
    this._refreshChart();
  }
  
  void _setChartStrategy() {
    switch (this.chartDefinition.chartType) {
      case ChartType.Line:
        this._chartStrategy = LineChartStrategy(); break;
      case ChartType.Pie:
        this._chartStrategy = PieChartStrategy(); break;
      case ChartType.Bar:
      case ChartType.LanguageFrequency:
        this._chartStrategy = BarChartStrategy(); break;
      case ChartType.Scatter:
        this._chartStrategy = ScatterChartStrategy(); break;
      case ChartType.PostsFrequency:
        this._chartStrategy = PostsFrequencyStrategy(); break;
    }
  }

  void _refreshChart() async {
    try {
      this.loadingData = true;
      var dataWithCount = await this._socnetoService.getChartData(this.jobId, this.chartDefinition, this.PAGE_SIZE, this.currentPage);
      this.chartData = dataWithCount.item1;
      this.totalResults = dataWithCount.item2;

      this.maxPage = (this.totalResults - 1) ~/ PAGE_SIZE + 1;
      this.isPaginated = this.chartDefinition?.chartType == ChartType.Line && this.maxPage > 1;

    } on HttpException catch (e) {
      Toastr.httpError(e);
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
