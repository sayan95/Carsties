using AuctionService.Models.DTOs;
using Carsties.Shared.Models.Core;

namespace AuctionService.Services;

public interface IAuctionService
{
    Task<Result<IReadOnlyList<AuctionDto>>> GetAllAuctions(string date, string correlationId);
    Task<Result<AuctionDto>> GetAuction(string id, string correlationId);
    Task<Result<AuctionDto>> CreateAuction(CreateAuctionDto createAuction, string correlationId);
    Task<Result<bool>> UpdateAuction(string id, UpdateAuctionDto updateAuction, string correlationId);
}
