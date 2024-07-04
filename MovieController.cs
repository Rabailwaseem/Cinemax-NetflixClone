using CinemaxFinal.Data;
using CinemaxFinal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CinemaxMVC.Controllers
{
    public class MovieController : Controller
    {
       
        private string connectionString;
        public MovieController(ApplicationDbContext context)
        {
           
            connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CinemaxDB;Integrated Security=True;";
        }


        [HttpPost]
        public IActionResult Play(int movieId)
        {
            MovieRepository movieRepository = new MovieRepository();
            movieRepository.AddView(movieId);

            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);
            var movie = genericRepo.SearchById(movieId);

            return View(movie);
        }
        [HttpPost]
        public IActionResult Like(int movieId)
        {
            MovieRepository movieRepository = new MovieRepository();
            movieRepository.AddLikes(movieId);

            return Json(new { success = true, responseText = "Yeahhh, you liked this movie!" });
        }




        public ViewResult Movies()
        {
            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);

            return View(genericRepo.GetAll());

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


        [HttpPost]
        public IActionResult MoreInfo(int movieId)
        {
            MovieRepository movieRepo=new MovieRepository();
            Movie movie=movieRepo.GetMovieDetails(movieId);
            return PartialView("_MoviesDetailModal", movie);
        }



        [HttpPost]
        public IActionResult SearchResults()
        {
            string q = Request.Form["query"];
            MovieRepository movieRepo = new MovieRepository();

            return View(movieRepo.SearchDB(q));

        }


      
        public ViewResult NewAndPopular()
        {
            MovieRepository movieRepository = new MovieRepository();
            return View(movieRepository.NewAndPopularDB());
        }
    }
}



