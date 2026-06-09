using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using WebApplication1.DTOS.Request_DTOs;
using WebApplication1.Entitys;
using WebApplication1.Services.AddressService;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController , Authorize]
    [EnableRateLimiting("UserActivityPolicy")]
    public class AddressController : ControllerBase 
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }
        [HttpPost("Create-Address")] 
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequestDto requestDto)
        {
            var UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var Result = await _addressService.CreateAddressAsync(requestDto, UserId);

            return StatusCode(Result.StatusCode, Result);
        }
        [HttpPut("Update-Address/{addressId}")]
        public async Task<IActionResult> UpdateAddress([FromRoute] int addressId, [FromBody] UpdateAddressRequestDto requestDto)
        {
            var UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var Result = await _addressService.UpdateAddressAsync(requestDto, addressId, UserId);
            
            return StatusCode(Result.StatusCode, Result);
        }

        [HttpDelete("Delete-Address/{addressId}")]
        public async Task<IActionResult> DeleteAddress([FromRoute] int addressId)
        {
            var UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var Result = await _addressService.DeleteAddressAsync( addressId, UserId);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpGet("Get-Address/{addressId}")]
        public async Task<IActionResult> GetAddressById([FromRoute] int addressId)
        {
            var UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var Result = await _addressService.GetAddressByIdAsync(addressId, UserId);

            return StatusCode(Result.StatusCode, Result);
        }

        [HttpGet("My-Addresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var Result = await _addressService.GetUserAddressesAsync(UserId);

            return StatusCode(Result.StatusCode, Result);
        }
    }
}
