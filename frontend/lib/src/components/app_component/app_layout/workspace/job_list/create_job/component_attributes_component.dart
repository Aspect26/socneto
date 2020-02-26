import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';
import 'package:angular_components/focus/focus_item.dart';
import 'package:angular_components/focus/focus_list.dart';
import 'package:angular_components/material_icon/material_icon.dart';
import 'package:angular_components/material_list/material_list.dart';
import 'package:angular_components/material_list/material_list_item.dart';
import 'package:angular_components/material_select/material_select_item.dart';
import 'package:angular_components/utils/angular/scroll_host/angular_2.dart';
import 'package:sw_project/src/components/shared/component_select/components_select_component.dart';
import 'package:sw_project/src/models/SocnetoComponent.dart';


@Component(
  selector: 'component-attributes',
  directives: [
    DeferredContentDirective,
    FocusItemDirective,
    FocusListDirective,
    MaterialIconComponent,
    MaterialListComponent,
    MaterialListItemComponent,
    MaterialSelectItemComponent,
    MaterialDropdownSelectComponent,
    MaterialRadioComponent,
    MaterialRadioGroupComponent,
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
    MaterialTabComponent,
    ModalComponent,
    ComponentsSelectComponent,
    MaterialStepperComponent,
    StepDirective,
    SummaryDirective,
    MaterialToggleComponent,
    materialInputDirectives,
    NgFor,
    NgIf,
  ],
  templateUrl: 'component_attributes_component.html',
  styleUrls: ['component_attributes_component.css'],
  providers: [materialProviders, scrollHostProviders, overlayBindings],
)
class ComponentAttributesComponent implements AfterChanges {

  @Input() List<SocnetoComponent> acquirers = [];
  @Input() bool disabled = false;

  final _attributesChangeController = StreamController<List<AcquirerWithAttributes>>();
  @Output() Stream<List<AcquirerWithAttributes>> get onAttributesChange => _attributesChangeController.stream;

  List<AcquirerWithAttributes> acquirersWithAttributes = [];
  AcquirerWithAttributes selectedAcquirer;

  @override
  void ngAfterChanges() {
    this.acquirersWithAttributes = [];
    this.acquirers.forEach((acquirer) {
      this.acquirersWithAttributes.add(AcquirerWithAttributes(acquirer, [MutableTuple("", "")]));
    });

    this.selectedAcquirer = this.acquirersWithAttributes.isNotEmpty? this.acquirersWithAttributes[0] : null;
  }

  void onUseTranslationChange(List<MutableTuple> attributes, bool translate) {
    this._setAttribute(attributes, "Translate", translate.toString());
  }

  void onRemove(List<MutableTuple> attributes, MutableTuple attribute) {
    attributes.remove(attribute);
    this._attributesChangeController.add(this.acquirersWithAttributes);
  }

  void onAddNew(List<MutableTuple> attributes) =>
      this._addNewEmpty(attributes);
  
  void onAttributeKeyChange(MutableTuple attribute, String key) {
    attribute.item1 = key;
    this._attributesChangeController.add(this.acquirersWithAttributes);
  }

  void onAttributeValueChange(MutableTuple attribute, String value) {
    attribute.item2 = value;
    this._attributesChangeController.add(this.acquirersWithAttributes);
  }

  void _addNewEmpty(List<MutableTuple> attributes) {
    attributes.add(MutableTuple<String, String>("", ""));
    this._attributesChangeController.add(this.acquirersWithAttributes);
  }

  void _setAttribute(List<MutableTuple> attributes, String key, String value) {
    var attribute = attributes.firstWhere((attribute) => attribute.item1 == key, orElse: () => null);
    if (attribute == null) {
      var newAttribute = MutableTuple(key, value);
      attributes.add(newAttribute);
    } else {
      attribute.item2 = value;
    }
    this._attributesChangeController.add(this.acquirersWithAttributes);
  }

}

class MutableTuple<T1, T2> {
  T1 item1;
  T2 item2;

  MutableTuple(this.item1, this.item2);
}

class AcquirerWithAttributes {
  SocnetoComponent acquirer;
  List<MutableTuple<String, String>> attributes;

  AcquirerWithAttributes(
      this.acquirer,
      this.attributes
  );
}