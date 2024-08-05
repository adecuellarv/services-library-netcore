using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApplicationApi.Application;
using WebApplicationApi.Models;

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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BookController>
        [HttpPost]
        public async Task<ActionResult<Unit>> Create([FromForm] AddBook.ExecuteBook data)
        {
            if (data.BookImageFile == null || data.BookPdfFile == null)
                return BadRequest("Los archivos no pueden ser nulos.");

            // path
            var directoryPath = @"C:\Users\Usuario\Documents\projects\files-exam";
            var bookImagePath = Path.Combine(directoryPath, data.BookImageFile.FileName);
            var bookPdfPath = Path.Combine(directoryPath, data.BookPdfFile.FileName);

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
            data.BookImage = bookImagePath;
            data.BookPdf = bookPdfPath;

            return await _mediator.Send(data);
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
