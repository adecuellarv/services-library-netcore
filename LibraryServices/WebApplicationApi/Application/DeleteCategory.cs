using FluentValidation;
using MediatR;
using Npgsql;
using System.Data;

namespace WebApplicationApi.Application
{
    public class DeleteCategory
    {
        public class CategoryDeleteID : IRequest
        {
            public string CategoryID { get; set; }
        }

        public class ExecuteValidations : AbstractValidator<CategoryDeleteID>
        {
            public ExecuteValidations()
            {
                RuleFor(x => x.CategoryID).NotEmpty();
            }
        }

        public class Managment : IRequestHandler<CategoryDeleteID, Unit> 
        {
            private readonly NpgsqlConnection _connection;
            private readonly ILogger<Managment> _logger;

            public Managment(NpgsqlConnection connection, ILogger<Managment> logger)
            {
                _connection = connection;
                _logger = logger;
            }

            public async Task<Unit> Handle(CategoryDeleteID request, CancellationToken cancellationToken)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync(cancellationToken);
                }

                try
                {

                    using (var command = new NpgsqlCommand("SELECT public.deletecategory(@p_categoryid)", _connection))
                    {
                        //command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_categoryid", int.Parse(request.CategoryID));
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al eliminar la categoría: {Message}", ex.Message);
                    throw new Exception("Error al eliminar la categoría", ex);
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
