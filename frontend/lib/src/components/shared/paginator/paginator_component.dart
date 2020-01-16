import 'dart:async';

import 'package:angular/angular.dart';
import 'package:angular_components/angular_components.dart';

import 'Paginator.dart';


@Component(
  selector: 'paginator',
  directives: [
    NgFor,
    NgIf,
    NgClass,
  ],
  templateUrl: 'paginator_component.html',
  styleUrls: ['paginator_component.css'],
  providers: [materialProviders, overlayBindings],
)
class PaginatorComponent {

  @Input() Paginator paginator;
  @Input() int pages = 2;

  final _pageClickController = StreamController<int>();
  @Output() Stream<int> get pageChange => _pageClickController.stream;

  List pageLinks() {
    var firstPage = this.paginator.currentPage - this.pages;
    var lastPage = this.paginator.currentPage + this.pages;

    if (firstPage < 1) {
      firstPage = 1;
    }

    if (lastPage > this.paginator.maxPage()) {
      lastPage = this.paginator.maxPage();
    }

    var pageIndexes = [for(var i = firstPage; i <= lastPage; i += 1) i];
    return pageIndexes;
  }

  void onPreviousPageClick() {
    this.onPageClick(this.paginator.currentPage - 1);
  }

  void onNextPageClick() {
    this.onPageClick(this.paginator.currentPage + 1);
  }

  void onPageClick(int pageNumber) {
    this._pageClickController.add(pageNumber);
  }

}