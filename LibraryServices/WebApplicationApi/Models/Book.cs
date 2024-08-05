namespace WebApplicationApi.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookGuid { get; set; }
        public string BookName { get; set; }
        public string BookDescription { get; set; }
        public string BookImage {get; set; }
        public string BookPdf { get; set; }

        public IFormFile BookImageFile { get; set; }
        public IFormFile BookPdfFile { get; set; }
        public int Category { get; set; }
    }
}
