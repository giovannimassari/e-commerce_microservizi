using Magazzino.Business.Services;
using Magazzino.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Magazzino.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController(IStockService stockService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var stocks = await stockService.GetAllAsync(cancellationToken);
        return Ok(stocks);
    }

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var stock = await stockService.GetByProductIdAsync(productId, cancellationToken);

        if (stock is null)
            return NotFound(new { message = $"Nessuno stock trovato per il prodotto {productId}." });

        return Ok(stock);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrIncrement(
        [FromBody] CreaStockRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var stock = await stockService.CreateOrIncrementAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetByProductId), new { productId = stock.ProductId }, stock);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{productId:guid}")]
    public async Task<IActionResult> AggiornaQuantita(
        Guid productId,
        [FromBody] AggiornaStockRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var stock = await stockService.AggiornaQuantitaAsync(productId, request, cancellationToken);
            return Ok(stock);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}