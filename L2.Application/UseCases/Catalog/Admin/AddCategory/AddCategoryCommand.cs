using L2.Application.Abstractions;

namespace L2.Application.UseCases.Catalog.Admin.AddCategory;

public record AddCategoryCommand(string Name, Guid? ParentId) : ICommand<Guid>;