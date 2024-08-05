namespace WebApplicationApi.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookGuid { get; set; }
        public string BookName { get; set; }
        public string BookDescription { get; set; }
        public IFormFile BookImage {get; set; }
        public IFormFile BookPdf { get; set; }
        public Category Category { get; set; }
    }
}
