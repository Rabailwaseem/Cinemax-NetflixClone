using CinemaxFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CinemaxMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SignInManager<MyAppUser> _signInManager;

        private string connectionString;

        public HomeController( ILogger<HomeController> logger, SignInManager<MyAppUser> signInManager)
        {
         connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CinemaxDB;Integrated Security=True;";

         _signInManager = signInManager;
        
        _logger = logger;
       }



        //[Authorize]
        public IActionResult Home()
        {

            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);

            return View(genericRepo.GetAll());
        }

       
     



        public ViewResult Notifications()
        {
            return View();
        }
        public ViewResult UserProfile()
        {
            return View();
        }

        public IActionResult Search()
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

      
        public ViewResult Index()
        {
            return View();
        }


        public ViewResult Movies()
        {
            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);

            return View(genericRepo.GetAll());

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
        public IActionResult AddToList(int movieId)
        {
            // Get the current logged-in user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "You need to be logged in to add movies to your list.";
                return RedirectToAction("Index");
            }

            MovieRepository movieRepository = new MovieRepository();

            // Check if the movie already exists in the user's list
            if (movieRepository.MovieExistsInUserList(userId, movieId))
            {
                TempData["InfoMessage"] = "This movie is already in your list.";
            }
            else
            {
                movieRepository.AddToList(userId, movieId);
                TempData["SuccessMessage"] = "Yeahhh! Movie Added to list.";
            }

            return RedirectToAction("MyList", "Home"); // Redirect to the appropriate view
        }



        [HttpPost]

        public IActionResult RemoveFromList(int movieId)
        {
            // Get the current logged-in user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "You need to be logged in to add movies to your list.";
                return RedirectToAction("Index");
            }

            MovieRepository movieRepository = new MovieRepository();
            movieRepository.RemoveFromList(userId, movieId);
            TempData["SuccessMessage"] = "Removed From List.";

            return RedirectToAction("MyList", "Home"); // Redirect to the appropriate view
        }







        public ViewResult NewAndPopular()
        {
            MovieRepository movieRepository = new MovieRepository();
            return View(movieRepository.NewAndPopularDB());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

   
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Redirect to a specific action/view after logout
        }


    }
}
