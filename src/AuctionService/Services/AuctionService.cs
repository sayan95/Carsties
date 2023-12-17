using AuctionService.Data;
using AuctionService.Entities;
using AuctionService.Models.DTOs;
using AutoMapper;
using Carsties.Shared.Extensions.Logger;
using Carsties.Shared.Models.Core;
using Carsties.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Services;

public class AuctionService : IAuctionService
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly AuctionDbContext _context;

    public AuctionService(ILogger logger, IMapper mapper, AuctionDbContext context)
    {
        _logger = logger;
        _mapper = mapper;
        _context = context;
    }

    public async Task<Result<IReadOnlyList<AuctionDto>>> GetAllAuctions(string correlationId)
    {
        _logger.Here().MethodEnterd();
        _logger.Here().WithCorrelationId(correlationId)
            .Information("Request - get all auctions");

        var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        if (!auctions.Any())
        {
            _logger.Here().Warning("No auctions found in database");
            return Result<IReadOnlyList<AuctionDto>>.Failure(ErrorCodes.NotFound);
        }

        var result = _mapper.Map<IReadOnlyList<AuctionDto>>(auctions);

        _logger.Here().Information("Total {@count} auctions found", auctions.Count);
        _logger.Here().MethodExited();
        return Result<IReadOnlyList<AuctionDto>>.Success(result);
    }

    public async Task<Result<AuctionDto>> GetAuction(string id, string correlationId)
    {
        _logger.Here().MethodEnterd();
        _logger.Here().WithCorrelationId(correlationId)
            .Information("Request - get all auctions");

        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

        if (auction is null)
        {
            _logger.Here().WithCorrelationId(correlationId)
                .Warning("No auction was found in database");
            return Result<AuctionDto>.Failure(ErrorCodes.NotFound);
        }

        var result = _mapper.Map<AuctionDto>(auction);

        _logger.Here().WithCorrelationId(correlationId)
            .Information("Actions found {@auction}", result);
        _logger.Here().MethodExited();
        return Result<AuctionDto>.Success(result);
    }

    public async Task<Result<AuctionDto>> CreateAuction(CreateAuctionDto createAuction, string correlationId)
    {
        _logger.Here().MethodEnterd();
        _logger.Here()
            .WithCorrelationId(correlationId)
            .Information("Request -  create new auction {@auction}", createAuction);

        var auctionEntity = _mapper.Map<Auction>(createAuction);
        auctionEntity.Seller = "test";

        _context.Add(auctionEntity);
        var createResult = await _context.SaveChangesAsync() > 0;

        if (!createResult)
        {
            _logger.Here().WithCorrelationId(correlationId).Error("{category} - Could not save changes to the db", ErrorCodes.OperationFailed);
            return Result<AuctionDto>.Failure(ErrorCodes.OperationFailed);
        }

        _logger.Here().WithCorrelationId(correlationId).Information("New auction created successfully with id {id}", auctionEntity.Id);
        _logger.Here().MethodExited();
        return Result<AuctionDto>.Success(_mapper.Map<AuctionDto>(auctionEntity));
    }

    public async Task<Result<bool>> UpdateAuction(string id, UpdateAuctionDto updateAuction, string correlationId)
    {
        _logger.Here().MethodEnterd();
        _logger.Here().WithCorrelationId(correlationId)
            .Information("Request - update auction of {id} - {@updateAuction}", id, updateAuction);

        var auctionEntity = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

        if (auctionEntity is null)
        {
            _logger.Here().WithCorrelationId(correlationId)
                .Warning("No auction found with id {id}", id);
            return Result<bool>.Failure(ErrorCodes.NotFound);
        }

        _mapper.Map(updateAuction, auctionEntity, typeof(UpdateAuctionDto), typeof(Auction));

        _context.Entry(auctionEntity).State = EntityState.Modified;
        var updateResult = await _context.SaveChangesAsync() > 0;

        if (!updateResult)
        {
            _logger.Here().WithCorrelationId(correlationId).Error("{category} - Could not save changes to the db", ErrorCodes.OperationFailed);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        _logger.Here().WithCorrelationId(correlationId).Information("Auction updated successfully");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }
}