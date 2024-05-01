using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication.Controllers;

namespace WebApplication.Controllers
{
    public class Movie
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }
        public string Rating { get; set; }
    }

    public class MoviesController : ApiController
    {
        private static readonly List<Movie> _movies = new List<Movie>
        {
            new Movie
            {
                MovieID = 1,
                Title = "The Shawshank Redemption",
                ReleaseDate = new DateTime(1994, 10, 14),
                Genre = "Drama",
                Rating = "R"
            },
            new Movie
            {
                MovieID = 2,
                Title = "The Godfather",
                ReleaseDate = new DateTime(1972, 3, 24),
                Genre = "Crime, Drama",
                Rating = "R"
            },
            new Movie
            {
                MovieID = 3,
                Title = "The Dark Knight",
                ReleaseDate = new DateTime(2008, 7, 18),
                Genre = "Action, Crime, Drama",
                Rating = "PG-13"
            },
            new Movie
            {
                MovieID = 4,
                Title = "Schindler's List",
                ReleaseDate = new DateTime(1993, 12, 15),
                Genre = "Biography, Drama, History",
                Rating = "R"
            },
            new Movie
            {
                MovieID = 5,
                Title = "The Lord of the Rings: The Return of the King",
                ReleaseDate = new DateTime(2003, 12, 17),
                Genre = "Action, Adventure, Drama",
                Rating = "PG-13"
            }
        };

        [System.Web.Http.Route("api/movies/all")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetMovies()
        {
            return Ok(_movies);
        }

        [System.Web.Http.Route("api/movies/add")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddMovie([FromBody] Movie movie)
        {
            if (movie == null)
            {
                return BadRequest("Invalid movie data");
            }

            movie.MovieID = _movies.Count + 1;


            _movies.Add(movie);


            return Ok(movie);
        }

        [System.Web.Http.Route("api/movies/update/{id}")]
        [System.Web.Http.HttpPut]
        public IHttpActionResult UpdateMovie(int id, [FromBody] Movie movie)
        {
            if (movie == null)
            {
                return BadRequest("Invalid movie data");
            }

            Movie existingMovie = _movies.FirstOrDefault(m => m.MovieID == id);
            if (existingMovie == null)
            {
                return NotFound();
            }

            existingMovie.Title = movie.Title;
            existingMovie.ReleaseDate = movie.ReleaseDate;
            existingMovie.Genre = movie.Genre;
            existingMovie.Rating = movie.Rating;

            return Ok(existingMovie);
        }

        [System.Web.Http.Route("api/movies/delete/{id}")]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            Movie movieToDelete = _movies.FirstOrDefault(m => m.MovieID == id);
            if (movieToDelete == null)
            {
                return NotFound();
            }
            _movies.Remove(movieToDelete);

            return Ok(movieToDelete);
        }

        [System.Web.Http.Route("api/movies/search")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult Search(string movieName)
        {
            Movie movie = _movies.FirstOrDefault(m => m.Title.ToLower() == movieName.ToLower());
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [System.Web.Http.Route("api/movies/filter")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult FilterMoviesByRating(string rating)
        {
 
            IEnumerable<Movie> moviesWithRating = _movies.Where(m => m.Rating.ToLower() == rating.ToLower());
            if (!moviesWithRating.Any())
            {
                return NotFound();
            }

            return Ok(moviesWithRating);
        }
    }
}
