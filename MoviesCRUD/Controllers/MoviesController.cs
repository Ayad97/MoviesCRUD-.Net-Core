using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesCRUD.Context;
using MoviesCRUD.Models;
using MoviesCRUD.MoviesViewModel;
using NToastNotify;

namespace MoviesCRUD.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

       
        public MoviesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }

      public async Task<IActionResult> Index()
        {
            var result =await _context.Movies.OrderByDescending(m=>m.Rate).ToListAsync();
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var MoviesModel = new MovieViewModel
            {
                genres=await _context.Genres.OrderBy(m=>m.Name).ToListAsync()
            };
            return View("MoviesForm",MoviesModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                
                return View("MoviesForm",model);
            }

            var file = Request.Form.Files;
            if (!file.Any())
            {
                model.genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Please Enter Poster");
                return View("MoviesForm",model);
            }

            var Poster = file.FirstOrDefault();
            var allowExtention = new List<string> { ".jpg", "png" };
            if (!allowExtention.Contains(Path.GetExtension(Poster.FileName).ToLower()))
            {
                model.genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Only Poster PNG OR JPG");
                return View("MoviesForm",model);
            }
            if (Poster.Length > 1048576)
            {
                model.genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                ModelState.AddModelError("Poster", "Poster Size Cannot be More than 1MB");
                return View("MoviesForm",model);
            }
            using var dataStream = new MemoryStream();
            await Poster.CopyToAsync(dataStream);
            var movies = new Movies
            {
                GenreId = model.GenreId,
                Title = model.Title,
                Year = model.Year,
                Rate =model.Rate,
                StoreLine=model.StoreLine,
                Poster = dataStream.ToArray()

            };
            _context.Movies.Add(movies);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movies Added Successfully");
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            
                return BadRequest();
            
            var movieResult = await _context.Movies.FindAsync(id);

            if(movieResult == null)
            
                return NotFound();

            var MoviesModel = new MovieViewModel
            {
                Id = movieResult.Id,
                Title = movieResult.Title,
                GenreId=movieResult.GenreId,
                Year = movieResult.Year,
                Poster=movieResult.Poster,
                StoreLine=movieResult.StoreLine,
                Rate=movieResult.Rate,
                genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync()
            };

            return View("MoviesForm",MoviesModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(MovieViewModel moviesView)
        {
            if (!ModelState.IsValid)
            {
                moviesView.genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();

                return View("MoviesForm", moviesView);
            }

            var movieResult = await _context.Movies.FindAsync(moviesView.Id);

            if (moviesView == null)
                return NotFound();
            var file = Request.Form.Files;
            if (file.Any())
            {
                
                var Poster = file.FirstOrDefault();
                using var dataStream = new MemoryStream();
                await Poster.CopyToAsync(dataStream);
                moviesView.Poster = dataStream.ToArray();
               



                var allowExtention = new List<string> { ".jpg", "png" };
                if (!allowExtention.Contains(Path.GetExtension(Poster.FileName).ToLower()))
                {
                    moviesView.genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Only Poster PNG OR JPG");
                    return View("MoviesForm", moviesView);
                }
                if (Poster.Length > 1048576)
                {
                    moviesView.genres = await _context.Genres.OrderBy(m => m.Name).ToListAsync();
                    ModelState.AddModelError("Poster", "Poster Size Cannot be More than 1MB");
                    return View("MoviesForm", moviesView);
                }
                movieResult.Poster = moviesView.Poster;
            }

            movieResult.Title = moviesView.Title;
            movieResult.GenreId = moviesView.GenreId;
            movieResult.Year = moviesView.Year; 
            movieResult.StoreLine = moviesView.StoreLine;
            movieResult.Rate = moviesView.Rate;


            
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movies Edited Successfully");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
           if(id == null)
                return BadRequest();
           var movie =await _context.Movies.Include(m=> m.Genre).SingleOrDefaultAsync(m=>m.Id == id);
            if(movie == null)  
                return NotFound();

           return View(movie);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id==null)
                return BadRequest();
            var movie = await _context.Movies.FindAsync(id);
            if(movie==null) 
                return NotFound();

            return View(movie);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(MovieViewModel viewModel)
        {
            var movie = await _context.Movies.FindAsync(viewModel);
            if (movie == null)
                return NotFound();
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Movie Removed Successfully");
            return RedirectToAction(nameof(Index));

        }
    }
}
