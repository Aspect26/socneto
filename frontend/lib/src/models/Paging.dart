class Paging {

  final int totalSize;
  final int page;
  final int pageSize;

  Paging(this.totalSize, this.page, this.pageSize);

  Paging.fromMap(Map data) :
        totalSize = data["total_size"] ?? 0,
        page = data["page"] ?? 0,
        pageSize = data["page_size"] ?? 0;

}
