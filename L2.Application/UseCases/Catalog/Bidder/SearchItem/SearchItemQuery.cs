using L2.Application.Abstractions;
using Sieve.Models;

namespace L2.Application.UseCases.Catalog.Bidder.SearchItem;

public record SearchItemQuery(SieveModel SieveModel) : IQuery<SearchItemResult>;
