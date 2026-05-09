using Domain.Features.Payments.Entities;

namespace Domain.Features.Payments.Repository;

public interface IDriverTransferRepository
{
    Task<DriverTransfer?> GetByIdAsync(long id);
    Task<DriverTransfer?> GetByOrderIdAsync(long orderId);
    Task<DriverTransfer?> GetByAsaasTransferIdAsync(string asaasTransferId);
    Task<DriverTransfer> SaveAsync(DriverTransfer transfer);
    Task UpdateAsync(DriverTransfer transfer);
}