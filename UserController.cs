
using CinemaxFinal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CinemaxMVC.Controllers
{
    public class UserController : Controller
    {
        public ViewResult SignUp()
        {
            return View();
        }

        public ViewResult UserProfile()
        {
            return View();
        }

        public ViewResult UpdateProfile()
        {
            return View();
        }



        public ViewResult MyList()
        {
            // Set the cookie options
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            string data = string.Empty;

            // Get the current user's ID and name
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userName = HttpContext.User.Identity.Name;

            // Greet the user based on whether it's their first visit
            data = "Hi " + userName;
            if (HttpContext.Request.Cookies.ContainsKey("first_request"))
            {
                data = "Welcome Back " + userName;
            }
            else
            {
                HttpContext.Response.Cookies.Append("first_request", DateTime.Now.ToString(), cookieOptions);
            }

            // Get the user's movie list from the repository
            MovieRepository movieRepository = new MovieRepository();
            List<Movie> list = movieRepository.GetUserMovies(userId);

            // Create a tuple containing the user's ID, name, movie list, and cookie data
            var viewModel = Tuple.Create(userId, userName, list, data);

            return View(viewModel);
        }



        //[HttpPost]
        //public async Task<IActionResult> UpdateProfile(string oldPassword, string newPassword, string confirmPassword)
        //{
        //    // Validate the old password against the stored password in the database
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);
        //    var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        //    if (!result.Succeeded)
        //    {
        //        // Password change failed, handle errors
        //        foreach (var error in result.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //        return View(); // Return to the update profile view with error messages
        //    }

        //    // Password change successful, redirect to a success page or perform any other action
        //    return RedirectToAction("Index", "Home");
        //}






    }
}
