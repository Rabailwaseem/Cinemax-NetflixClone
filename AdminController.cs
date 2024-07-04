using CinemaxFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace CinemaxFinal.Controllers
{

  //  [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {

       // private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CinemaxDB;Integrated Security=True;";
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AdminController> _logger;
        private readonly IRepository<Movie> _movieRepo;
        private readonly  string connectionString;
        public AdminController(IWebHostEnvironment env,ILogger<AdminController> logger)
        {
             connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CinemaxDB;Integrated Security=True;";
            _movieRepo = new GenericRepository<Movie>(connectionString);
            _env = env;
            _logger= logger;
        }

        [HttpPost]
        public IActionResult AddMovieInDB(Movie movie)
        {
         
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(wwwrootPath, "UploadedFiles");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (movie.ImageFile.Length > 0)
            {

                string filePath = Path.Combine(path, movie.ImageFile.FileName);

                using (var FileStream = new FileStream(filePath, FileMode.Create))
                {
                    movie.ImageFile.CopyTo(FileStream);
                }
                string ImagePathInRoot = "/UploadedFiles/" + movie.ImageFile.FileName;
                movie.ImagePath = ImagePathInRoot;
            }


            // Handle VideoFile upload
            if (movie.VideoFile?.Length > 0)
            {
                string videoFilePath = Path.Combine(path, movie.VideoFile.FileName);

                using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
                {
                    movie.VideoFile.CopyTo(fileStream);
                }

                string videoPathInRoot = "/UploadedFiles/" + movie.VideoFile.FileName;
                movie.VideoPath = videoPathInRoot;
            }

            // Add the movie to the repository
            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);
            genericRepo.Add(movie);


            // Return a success message
            return Content("Added");

            //return RedirectToAction("AddMovie","Admin");
        }
        public ViewResult AddMovie()
        {
            return View();
        }

        public ViewResult DeleteMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteMovie(int id)
        {
           Movie movie= new Movie();
            movie.Id = id;
            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);
            bool a= genericRepo.Delete(movie);

            if (a)
            {
                return Content("deleted");
            }
            else
            {
                return Content("not deleted");
            }
            

        }


        public ViewResult UpdateMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateMovieInDB(Movie movie)
        {
           
         
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(wwwrootPath, "UploadedFiles");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (movie.ImageFile.Length > 0)
            {

                string filePath = Path.Combine(path, movie.ImageFile.FileName);

                using (var FileStream = new FileStream(filePath, FileMode.Create))
                {
                    movie.ImageFile.CopyTo(FileStream);
                }
                string ImagePathInRoot = "/UploadedFiles/" + movie.ImageFile.FileName;
                movie.ImagePath = ImagePathInRoot;
            }


            if (movie.VideoFile != null && movie.VideoFile.Length > 0)
            {
                string videoFilePath = Path.Combine(path, movie.VideoFile.FileName);

                // Save the video file to the server
                using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
                {
                    movie.VideoFile.CopyTo(fileStream);
                }

                // Set the video path relative to the web root
                string videoPathInRoot = "/UploadedFiles/" + movie.VideoFile.FileName;
                movie.VideoPath = videoPathInRoot;
            }

            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);
            bool isUpdated = genericRepo.Update(movie);

            if (isUpdated)
            {
                return Content("Movie updated successfully");
            }
            else
            {
                return Content("Movie update failed");
            }
        }


        [AllowAnonymous]
        public IActionResult DisplayMovies()
        {
            GenericRepository<Movie> genericRepo = new GenericRepository<Movie>(connectionString);
            
            return View(genericRepo.GetAll());

        }


    }
}
