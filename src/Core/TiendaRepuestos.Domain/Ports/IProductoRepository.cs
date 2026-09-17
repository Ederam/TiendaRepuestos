namespace TiendaRepuestos.Domain.Ports;

using TiendaRepuestos.Domain.Entities;

public interface IProductoRepository
{
    Task<Producto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Producto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task AddAsync(Producto producto, CancellationToken cancellationToken = default);
    Task UpdateAsync(Producto producto, CancellationToken cancellationToken = default);
}