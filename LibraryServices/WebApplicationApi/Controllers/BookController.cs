using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApplicationApi.Application;
using WebApplicationApi.Models;
using static WebApplicationApi.Application.GetBooksFilter;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BookController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/<BookController>
        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetBooks()
        {
            return await _mediator.Send(new GetBooks.Books());
        }

        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<BookDto>>> GetByCat(string id)
        {
            return await _mediator.Send(new GetBooksFilter.BookByCat { CategoryId = id });
        }

        // POST api/<BookController>
        [HttpPost]
        public async Task<ActionResult<Unit>> Create([FromForm] AddBook.ExecuteBook data)
        {
            if (data.BookImageFile == null || data.BookPdfFile == null)
                return BadRequest("Los archivos no pueden ser nulos.");

            // path
            //var directoryPath = @"C:\Users\Usuario\Documents\projects\files-exam";
            //var directoryPath = Path.Combine("wwwroot", "uploads", )
            var bookImagePath = Path.Combine("wwwroot", "uploads", data.BookImageFile.FileName);
            var bookPdfPath = Path.Combine("wwwroot", "uploads", data.BookPdfFile.FileName);

            //upload
            using (var stream = new FileStream(bookImagePath, FileMode.Create))
            {
                await data.BookImageFile.CopyToAsync(stream);
            }

            using (var stream = new FileStream(bookPdfPath, FileMode.Create))
            {
                await data.BookPdfFile.CopyToAsync(stream);
            }

            // path to bd
            data.BookImage = $"{Request.Scheme}://{Request.Host}/uploads/{data.BookImageFile.FileName}"; // bookImagePath;
            data.BookPdf = $"{Request.Scheme}://{Request.Host}/uploads/{data.BookImageFile.FileName}"; //bookPdfPath;

            return await _mediator.Send(data);
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Update([FromForm] EditBook.ExecuteBook data, string id)
        {
            if (data.BookImageFile == null || data.BookPdfFile == null)
                return BadRequest("Los archivos no pueden ser nulos.");

            // path
            var bookImagePath = Path.Combine("wwwroot", "uploads", data.BookImageFile.FileName);
            var bookPdfPath = Path.Combine("wwwroot", "uploads", data.BookPdfFile.FileName);

            //upload
            using (var stream = new FileStream(bookImagePath, FileMode.Create))
            {
                await data.BookImageFile.CopyToAsync(stream);
            }

            using (var stream = new FileStream(bookPdfPath, FileMode.Create))
            {
                await data.BookPdfFile.CopyToAsync(stream);
            }

            // path to bd
            data.BookImage = $"{Request.Scheme}://{Request.Host}/uploads/{data.BookImageFile.FileName}"; // bookImagePath;
            data.BookPdf = $"{Request.Scheme}://{Request.Host}/uploads/{data.BookImageFile.FileName}"; //bookPdfPath;
            data.BookId = int.Parse(id);

            return await _mediator.Send(data);
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(string id)
        {
            return await _mediator.Send(new DeleteBook.BookDeleteID { BookID = id });
        }
    }
}
