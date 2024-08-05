using AutoMapper;
using MediatR;
using Npgsql;
using System.Data;
using static WebApplicationApi.Application.GetBooks;

namespace WebApplicationApi.Application
{
    public class GetBooksFilter
    {
        public class BookByCat : IRequest<List<BookDto>>
        {
            public string CategoryId { get; set; }
        }

        public class Managment : IRequestHandler<BookByCat, List<BookDto>>
        {
            private readonly NpgsqlConnection _connection;
            private readonly IMapper _mapper;
            private readonly ILogger<Managment> _logger;

            public Managment(NpgsqlConnection connection, IMapper mapper, ILogger<Managment> logger)
            {
                _connection = connection;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<List<BookDto>> Handle(BookByCat request, CancellationToken cancellationToken)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync(cancellationToken);
                }

                var bookList = new List<BookDto>();
                try
                {
                    using (var command = new NpgsqlCommand("SELECT * FROM public.books WHERE CategoryID = @CategoryID", _connection))
                    {
                        command.Parameters.AddWithValue("@CategoryID", int.Parse(request.CategoryId));
                        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                var book = new BookDto
                                {
                                    BookId = reader.GetInt32(0),
                                    BookName = reader.GetString(1),
                                    BookDescription = reader.GetString(2),
                                    BookImage = reader.GetString(3),
                                    BookPdf = reader.GetString(4),
                                    Category = reader.GetInt32(5)
                                };
                                bookList.Add(book);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al traer la lista de libros: {Message}", ex.Message);
                    throw new Exception("Error al traer la lista de libros", ex);
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        await _connection.CloseAsync();
                    }
                }
                return bookList;
            }
        }
    }
}