namespace L2.Application.Models;

public record Meta(
  int Page,
  int PageSize,
  int Total,
  int TotalPages,
  bool HasPagination,
  bool HasPreviousPage,
  bool HasNextPage
) {
  public static Meta Create(int page, int pageSize, int total) {
    page = page < 1 ? 1 : page;
    pageSize = pageSize < 1 ? 1 : pageSize;
    total = total < 0 ? 0 : total;

    var totalPages = (total + pageSize - 1) / pageSize;

    page = Math.Min(page, totalPages);
    var hasPagination = totalPages > 1;
    var hasPreviousPage = hasPagination && page > 1;
    var hasNextPage = hasPagination && page != totalPages;
    return new Meta(page, pageSize, total, totalPages, hasPagination, hasPreviousPage, hasNextPage);
  }
}
