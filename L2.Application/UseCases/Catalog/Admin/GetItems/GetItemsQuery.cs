using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.Admin.GetItems;

public record GetItemsQuery(SieveModel SieveModel) : IQuery<GetItemsResult>;