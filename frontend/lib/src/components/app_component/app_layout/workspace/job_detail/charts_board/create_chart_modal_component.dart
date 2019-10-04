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
    // TODO: move all the directives to some parent or something
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
    AutoDismissDirective,
    AutoFocusDirective,
    MaterialButtonComponent,
    MaterialTooltipDirective,
    MaterialDialogComponent,
    MaterialFabComponent,
    ModalComponent,
    ChartTypeSelectComponent,
    NgFor,
    NgIf,
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
  List<AnalysisDataPath> dataPaths = [];
  ChartType chartType;

  CreateChartModalComponent(this._socnetoService) {
    this.loadAnalyzers();
  }

  static final ItemRenderer itemRenderer = newCachingItemRenderer<dynamic>(
          (analyser) => "${analyser.identifier}");

  void loadAnalyzers() async {
    // TODO: this should ask for analyzers of the selected job
    this.analysers = await this._socnetoService.getAvailableAnalyzers();
  }

  void show() {
    this.reset();
    this.displayed = true;
  }

  void close() {
    this.displayed = false;
  }

  void reset() {
    this.dataPaths = [];
    this.onAddDataPath();

    this.errorMessage = null;
  }

  bool isShown() {
    return this.displayed;
  }

  void onCloseDialog() {
    this.displayed = false;
  }

  void onRemoveDataPath(AnalysisDataPath dataPath) {
    this.dataPaths.remove(dataPath);
  }

  void onAddDataPath() {
    if (this.analysers.isEmpty) {
      return;
    }

    final analyser = this.analysers[0];
    this.dataPaths.add(AnalysisDataPath(analyser, analyser.properties.isNotEmpty? analyser.properties[0] : ""));
  }

  void onSubmit() async {
    if (!this.isDefinitionCorrect()) {
      this.errorMessage = "Incorrect chart definition";
      return;
    }

    this._submitController.add(ChartDefinition(this.dataPaths.map((dataPath) => dataPath.toJsonPath()).toList(), this.chartType));
  }

  void onChartTypeSelected(ChartType chartType) {
    this.chartType = chartType;
  }

  bool isDefinitionCorrect() {
    return this.chartType != null && this.dataPaths.isNotEmpty;
  }

}