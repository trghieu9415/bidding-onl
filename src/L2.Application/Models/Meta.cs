namespace L2.Application.Models;

public record Meta(
  int Page,
  int PerPage,
  int Total,
  int TotalPages,
  bool HasPagination,
  bool HasPreviousPage,
  bool HasNextPage
) {
  public static Meta Create(int page, int perPage, int total) {
    page = page < 1 ? 1 : page;
    perPage = perPage < 1 ? 1 : perPage;
    total = total < 0 ? 0 : total;

    var totalPages = (total + perPage - 1) / perPage;

    page = Math.Min(page, totalPages);
    var hasPagination = totalPages > 1;
    var hasPreviousPage = hasPagination && page > 1;
    var hasNextPage = hasPagination && page != totalPages;
    return new Meta(page, perPage, total, totalPages, hasPagination, hasPreviousPage, hasNextPage);
  }
}
