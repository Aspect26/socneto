import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:sw_project/src/components/app_component/app_layout/workspace/job_detail/charts_board/chart_type_select/chart_type_select_component.dart';
import 'package:sw_project/src/models/AnalysisDataPath.dart';
import 'package:sw_project/src/models/ChartDefinition.dart';
import 'package:sw_project/src/models/Job.dart';
import 'package:sw_project/src/models/SocnetoAnalyser.dart';
import 'package:sw_project/src/services/socneto_service.dart';


@Component(
  selector: 'create-chart-modal',
  directives: [
    DeferredContentDirective,
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    MaterialDropdownSelectComponent,
    MaterialTabPanelComponent,
    MaterialTabComponent,
    MaterialRadioComponent,
    MaterialRadioGroupComponent,
    MaterialInputComponent,
    MaterialCheckboxComponent,
    AutoDismissDirective,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialTooltipDirective,
    MaterialDialogComponent,
    MaterialFabComponent,
    materialInputDirectives,
    ModalComponent,
    ChartTypeSelectComponent,
    NgFor,
    NgIf,
    NgClass
  ],
  templateUrl: 'create_chart_modal_component.html',
  styleUrls: ['create_chart_modal_component.css'],
  providers: [materialProviders, overlayBindings],
)
class CreateChartModalComponent {

  @Input() Job job;

  final _submitController = StreamController<ChartDefinition>();
  @Output() Stream<ChartDefinition> get submit => _submitController.stream;

  final SocnetoService _socnetoService;

  List<SocnetoAnalyser> analysers = [];
  bool displayed = false;
  String errorMessage;
  String chartTitle = "";
  List<AnalysisDataPath> dataPaths = [];
  ChartType chartType;
  bool useTimeAsX = false;

  CreateChartModalComponent(this._socnetoService) {
    this.loadAnalyzers();
  }

  List<String> get analyserIds => this.analysers?.map((analyser) => analyser.identifier)?.toList() ?? [];

  void loadAnalyzers() async {
    // TODO: this should ask for analyzers of the selected job
    var allAnalysers = await this._socnetoService.getAvailableAnalyzers();
    this.analysers = allAnalysers.where((analyser) => analyser.properties.isNotEmpty).toList();
  }

  void show() {
    this.reset();
    this.displayed = true;
  }

  void close() {
    this.displayed = false;
  }

  void reset() async {
    this.dataPaths.clear();
    this.useTimeAsX = false;

    await this.loadAnalyzers();
    var requiredNumberOfDataPaths = this.requiredNumberOfDataPaths;
    this._addDefaultDataPaths(requiredNumberOfDataPaths);

    this.errorMessage = null;
  }

  bool isShown() {
    return this.displayed;
  }

  void onCloseDialog() {
    this.displayed = false;
  }

  int get requiredNumberOfDataPaths {
    switch (this.chartType) {
      case ChartType.Line:
        return 2;
      case ChartType.Pie:
      case ChartType.Bar:
      case ChartType.Table:
        return 1;
      case ChartType.Scatter:
        return 2;
      case ChartType.LanguageFrequency:
      case ChartType.PostsFrequency:
      case ChartType.AuthorFrequency:
        return 0;
    }
  }

  int get maxNumberOfDataPaths {
    switch (this.chartType) {
      case ChartType.Line:
        return 100;
      case ChartType.Pie:
      case ChartType.Bar:
      case ChartType.Table:
        return 1;
      case ChartType.Scatter:
        return 2;
      case ChartType.LanguageFrequency:
      case ChartType.PostsFrequency:
      case ChartType.AuthorFrequency:
        return 0;
    }
  }

  ChartType get scatterChartType => ChartType.Scatter;
  ChartType get lineChartType => ChartType.Line;

  String getDataPathLabel(int index) {
    switch (this.chartType) {
      case ChartType.Line:
        return (index == 0)? "X - Axis" : "Line ${index}";
      case ChartType.Pie:
      case ChartType.Bar:
      case ChartType.Table:
        return "Analysis property";
      case ChartType.Scatter:
        return index == 0? "X - Axis" : "Y - Axis";
      case ChartType.PostsFrequency:
      case ChartType.LanguageFrequency:
      case ChartType.AuthorFrequency:
        return "";
    }
  }

  List<String> getAnalyserProperties(String analyserId) {
    var analyser = this.analysers.firstWhere((analyser) => analyser.identifier == analyserId, orElse: () => null);
    return analyser?.properties?.map((analyserProperty) => analyserProperty.name)?.toList() ?? [];
  }

  void onUseTimeAsXChange(bool checked) {
    this.useTimeAsX = checked;
  }

  void onRemoveDataPath(AnalysisDataPath dataPath) {
    this.dataPaths.remove(dataPath);
  }

  void onChartTypeSelected(ChartType chartType) {
    this.chartType = chartType;
    this.dataPaths.clear();
    this.useTimeAsX = false;

    var requiredNumberOfDataPaths = this.requiredNumberOfDataPaths;
    this._addDefaultDataPaths(requiredNumberOfDataPaths);
  }

  void onAddDataPath() {
    if (this.analysers.isEmpty || this.chartType == ChartType.Scatter) {
      return;
    }

    this._addDefaultDataPaths(1);
  }

  void onDataPathAnalyserChanged(AnalysisDataPath dataPath, SocnetoAnalyser analyser) {
    dataPath.analyserId = analyser.identifier;
    dataPath.property = analyser.properties[0].name;
  }

  void onSubmit() async {
    if (!this.isDefinitionCorrect()) {
      this.errorMessage = "Incorrect chart definition";
      return;
    }

    var usedDataPaths = this.useTimeAsX && this.chartType == ChartType.Line? this.dataPaths.sublist(1, this.dataPaths.length) : this.dataPaths;
    this._submitController.add(ChartDefinition(this.chartTitle, usedDataPaths, this.chartType, this.useTimeAsX));
  }

  bool isDefinitionCorrect() {
    return this.chartTitle != null && this.chartTitle.isNotEmpty && this.chartType != null
        && (this.dataPaths.length >= this.requiredNumberOfDataPaths)
        && (this.dataPaths.length <= this.maxNumberOfDataPaths);
  }

  void _addDefaultDataPaths(int count) {
    for (var i = 0; i < count; i++) {
      if (this.analysers.isNotEmpty) {
        final analyser = this.analysers[0];
        this.dataPaths.add(AnalysisDataPath(analyser.identifier, analyser.properties[0].name));
      }
    }
  }

}