using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOS;
using WebApplication1.Entitys;
using WebApplication1.Models;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        ResponsModel response = new();
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("Get-All-users")]
        public async Task<IActionResult> GetAll()
        {
            var AllUsers =await _userService.GetUsers();
            response.Status = true;
            response.StatusMessage = "those are all the users";
            response.Data = AllUsers;
            return Ok(response);
        }
        [HttpGet("Get-User-By-Id/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Result = await _userService.GetUserById(id);
            if (Result == null)
            {
                response.Status = false;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.StatusMessage = "The Data Was Found";
                response.Data = Result;
            }
            return Ok(response);
        }
        [HttpPut("Update-User")]
        public async Task<IActionResult> UpdateUser(int id , User user)
        {
            var result = await _userService.UpdateUser(id, user);
            if (result == null)
            {
                response.Status = false;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.StatusMessage = "The User has been successfully updated ";
                response.Data = result;
            }
            return Ok(response);

        }
        [HttpDelete("Delete-User/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var Delete = await _userService.DeleteUser(id);
            if (Delete == false)
            {
                response.Status = false;
                response.StatusMessage = "No Data Was Found";
            }
            else
            {
                response.Status = true;
                response.StatusMessage = "The User has been successfully Deleted ";
            }
            return Ok(response);
        }

    }
}
