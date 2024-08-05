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

            public Managment(NpgsqlConnection connection)
            {
                _connection = connection;
            }

            public async Task<Unit> Handle(CategoryDeleteID request, CancellationToken cancellationToken)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync(cancellationToken);
                }

                using (var command = new NpgsqlCommand("SELECT public.deletecategory(@p_categoryid)", _connection))
                {
                    //command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("p_categoryid", int.Parse(request.CategoryID));
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}
