// TODO: this should be in models :(
class Paginator {

  int entriesCount = 0;
  int currentPage = 0;
  int pageSize = 0;

  Paginator(this.entriesCount, this.currentPage, this.pageSize);

  int maxPage() {
    return pageSize != 0? (entriesCount - 1) ~/ pageSize + 1 : 1;
  }

}