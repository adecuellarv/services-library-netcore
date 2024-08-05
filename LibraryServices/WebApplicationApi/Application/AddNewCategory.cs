using FluentValidation;
using MediatR;
using Npgsql;
using System.Data;
using System.Xml.Linq;
using WebApplicationApi.Models;

namespace WebApplicationApi.Application
{
    public class AddNewCategory
    {
        public class ExecuteCategory : IRequest
        {
            public string CategoryName { get; set; }
        }

        public class ExecuteValidations : AbstractValidator<ExecuteCategory>
        {
            public ExecuteValidations() 
            { 
                RuleFor(x=> x.CategoryName).NotEmpty();
            }
        }

        public class Managment : IRequestHandler<ExecuteCategory, Unit>
        {
            private readonly NpgsqlConnection _connection;
            private readonly ILogger<Managment> _logger;

            public Managment(NpgsqlConnection connection, ILogger<Managment> logger)
            {
                _connection = connection;
                _logger = logger;
            }

            public async Task<Unit> Handle(ExecuteCategory request, CancellationToken cancellationToken)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync(cancellationToken);
                }

                try
                {
                    using (var command = new NpgsqlCommand("SELECT public.addcategory(@p_categoryname)", _connection))
                    {
                        //command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_categoryname", request.CategoryName);
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al agregar la categoría: {Message}", ex.Message);
                    throw new Exception("Error al agregar la categoría", ex);
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
