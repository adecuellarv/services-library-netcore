using FluentValidation;
using MediatR;
using Npgsql;
using System.Data;
using WebApplicationApi.Models;

namespace WebApplicationApi.Application
{
    public class AddBook
    {
        public class ExecuteBook : IRequest
        {
            public string BookName { get; set; }
            public string BookDescription { get; set; }
            public string BookImage { get; set; }
            public string BookPdf { get; set; }
            public IFormFile BookImageFile { get; set; }
            public IFormFile BookPdfFile { get; set; }
            public int Category { get; set; }
        }
        public class ExecuteValidations : AbstractValidator<ExecuteBook>
        {
            public ExecuteValidations()
            {
                RuleFor(x => x.BookName).NotEmpty();
                RuleFor(x => x.BookDescription).NotEmpty();
                RuleFor(x => x.BookImageFile).NotEmpty();
                RuleFor(x => x.BookPdfFile).NotEmpty();
                RuleFor(x => x.Category).NotEmpty();
            }
        }

        public class Managment : IRequestHandler<ExecuteBook, Unit>
        {
            private readonly NpgsqlConnection _connection;
            private readonly ILogger<Managment> _logger;

            public Managment(NpgsqlConnection connection, ILogger<Managment> logger)
            {
                _connection = connection;
                _logger = logger;
            }

            public async Task<Unit> Handle(ExecuteBook request, CancellationToken cancellationToken)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync(cancellationToken);
                }
                try
                {
 
                    using (var command = new NpgsqlCommand("SELECT public.addbook(@p_book_name, @p_book_description, @p_book_image, @p_book_pdf, @p_category_id)", _connection))
                    {
                        //command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_book_name", request.BookName);
                        command.Parameters.AddWithValue("p_book_description", request.BookDescription);
                        command.Parameters.AddWithValue("p_book_image", request.BookImage);
                        command.Parameters.AddWithValue("p_book_pdf", request.BookPdf);
                        command.Parameters.AddWithValue("p_category_id", request.Category);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al agregar libro: {Message}", ex.Message);
                    throw new Exception("Error al agregar libro", ex);
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        await _connection.CloseAsync();
                    }
                }

                return Unit.Value;
            }
        }
    }
}
