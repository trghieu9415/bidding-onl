using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.Bidder.GetRegisteredItems;

public record GetRegisteredItemsQuery(SieveModel SieveModel) : IQuery<GetRegisteredItemsResult>;