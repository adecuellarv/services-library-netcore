using FluentValidation;
using MediatR;
using Npgsql;
using System.Data;

namespace WebApplicationApi.Application
{
    public class DeleteBook
    {
        public class BookDeleteID : IRequest
        {
            public string BookID { get; set; }
        }

        public class ExecuteValidations : AbstractValidator<BookDeleteID>
        {
            public ExecuteValidations()
            {
                RuleFor(x => x.BookID).NotEmpty();
            }
        }
        public class Managment : IRequestHandler<BookDeleteID, Unit>
        {
            private readonly NpgsqlConnection _connection;
            private readonly ILogger<Managment> _logger;

            public Managment(NpgsqlConnection connection, ILogger<Managment> logger)
            {
                _connection = connection;
                _logger = logger;
            }

            public async Task<Unit> Handle(BookDeleteID request, CancellationToken cancellationToken)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync(cancellationToken);
                }

                try
                {

                    using (var command = new NpgsqlCommand("SELECT public.deletebook(@p_book_id)", _connection))
                    {
                        //command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_book_id", int.Parse(request.BookID));
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar libro: {Message}", ex.Message);
                    throw new Exception("Error al eliminar libro", ex);
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
