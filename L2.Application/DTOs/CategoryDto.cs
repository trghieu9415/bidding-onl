namespace L2.Application.DTOs;

public record CategoryDto(
  Guid Id,
  string Name,
  Guid? ParentId,
  DateTime CreatedAt
);