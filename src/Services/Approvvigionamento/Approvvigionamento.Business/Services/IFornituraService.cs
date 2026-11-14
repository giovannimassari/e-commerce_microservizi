using Approvvigionamento.Shared.DTOs;
using EventBus.Contracts.Events;

namespace Approvvigionamento.Business.Services;

public interface IFornituraService
{
    Task RegistraRichiestaDaScortaAsync(ScortaBassaEvent evento, CancellationToken cancellationToken = default);
    Task<List<RichiestaRifornimentoDto>> GetRichiesteAsync(string? stato, CancellationToken cancellationToken = default);
    Task<RichiestaRifornimentoDto?> GetRichiestaAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FornituraDto> CompletaRichiestaAsync(Guid richiestaId, CompletaRichiestaRequest request, CancellationToken cancellationToken = default);
    Task<List<FornituraDto>> GetFornitureAsync(CancellationToken cancellationToken = default);
}
