using Domain.Features.Payments.Entities;
using Domain.Features.Payments.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Postgreesql.Features.Payments.Repository;

/// <summary>
/// Repository implementation for DriverTransfer entity
/// </summary>
public class DriverTransferRepository : IDriverTransferRepository
{
    private readonly TILTContext _context;

    public DriverTransferRepository(TILTContext context)
    {
        _context = context;
    }

    public async Task<DriverTransfer?> GetByIdAsync(long id)
    {
        return await _context.DriverTransfers.FindAsync(id);
    }

    public async Task<DriverTransfer?> GetByOrderIdAsync(long orderId)
    {
        return await _context.DriverTransfers
            .FirstOrDefaultAsync(t => t.OrderId == orderId);
    }

    public async Task<DriverTransfer?> GetByAsaasTransferIdAsync(string asaasTransferId)
    {
        return await _context.DriverTransfers
            .FirstOrDefaultAsync(t => t.AsaasTransferId == asaasTransferId);
    }

    public async Task<DriverTransfer> SaveAsync(DriverTransfer transfer)
    {
        if (transfer.Id == 0)
        {
            await _context.DriverTransfers.AddAsync(transfer);
        }
        else
        {
            _context.DriverTransfers.Update(transfer);
        }

        return transfer;
    }

    public async Task UpdateAsync(DriverTransfer transfer)
    {
        _context.DriverTransfers.Update(transfer);
        await Task.CompletedTask;
    }
}