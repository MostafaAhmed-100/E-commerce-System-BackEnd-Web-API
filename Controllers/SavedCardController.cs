using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Constants;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Buyer + AppRoles.Admin)]
    public class SavedCardController : ControllerBase
    {
        private readonly ISavedCardService _savedCardService;

        public SavedCardController(ISavedCardService savedCardService)
        {
            _savedCardService = savedCardService;
        }

        [HttpPost("Add-Card")]
        public async Task<IActionResult> AddCard([FromBody] AddSavedCardRequestDto request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid Token or User ID." });
            }

            var response = await _savedCardService.AddCardAsync(request, userId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("My-Cards")]
        public async Task<IActionResult> GetMyCards()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid Token or User ID." });
            }

            var response = await _savedCardService.GetMyCardsAsync(userId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("Delete-Card/{cardId}")]
        public async Task<IActionResult> DeleteCard(int cardId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid Token or User ID." });
            }

            var response = await _savedCardService.DeleteCardAsync(cardId, userId);

            return StatusCode(response.StatusCode, response);
        }
    }
}