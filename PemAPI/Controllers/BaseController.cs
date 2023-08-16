using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PemAPI.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction] // So asp net middleware doesnt think its a route
        public int? GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                return null;
            }

            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
