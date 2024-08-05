namespace WebApplicationApi.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryGuid { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Book> Books { get; set;}
    }
}
