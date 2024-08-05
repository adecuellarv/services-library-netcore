using FluentValidation;
using MediatR;
using Npgsql;
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

            public Managment(NpgsqlConnection connection)
            {
                _connection = connection;
            }

            public async Task<Unit> Handle(ExecuteCategory request, CancellationToken cancellationToken)
            {
                await _connection.OpenAsync(cancellationToken);

                using (var command = new NpgsqlCommand("SELECT public.addcategory(@p_categoryname)", _connection))
                {
                    //command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_categoryname", request.CategoryName);
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}
