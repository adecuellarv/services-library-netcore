using AutoMapper;
using MediatR;
using Npgsql;
using System.Data;
using WebApplicationApi.Models;

namespace WebApplicationApi.Application
{
    public class GetCategories
    {
        public class Categories : IRequest<List<Category>> { }

        public class Managment : IRequestHandler<Categories, List<Category>> 
        {
            private readonly NpgsqlConnection _connection;
            private readonly IMapper _mapper;
            private readonly ILogger<Managment> _logger;
            public Managment(NpgsqlConnection connection, IMapper mapper, ILogger<Managment> logger) {
                _connection = connection;
                _logger = logger;
                _mapper = mapper;
            }

            public async Task<List<Category>> Handle(Categories request, CancellationToken cancellationToken)
            {
                if (_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync(cancellationToken);
                }
                var categoriesList = new List<Category>();

                try
                {
                    using (var command = new NpgsqlCommand("SELECT categoryid, categoryguid, categoryname FROM public.categories;", _connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                        {
                            while (await reader.ReadAsync(cancellationToken))
                            {
                                var category = new Category
                                {
                                    CategoryId = reader.GetInt32(0),
                                    CategoryGuid = reader.GetString(1),
                                    CategoryName = reader.GetString(2)
                                };
                                categoriesList.Add(category);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al traer categorias: {Message}", ex.Message);
                    throw new Exception("Error al traer categorias", ex);
                }
                finally
                {
                    if (_connection.State == ConnectionState.Open)
                    {
                        await _connection.CloseAsync();
                    }
                }
                return categoriesList;
            }
        }
    }
}
