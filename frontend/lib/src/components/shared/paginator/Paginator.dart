class Paginator {

  int currentPage = 0;
  int pageSize = 0;
  int entriesCount = 0;

  Paginator(this.entriesCount, this.currentPage, this.pageSize);

  int maxPage() {
    return entriesCount ~/ pageSize;
  }

}