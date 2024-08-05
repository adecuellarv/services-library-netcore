namespace WebApplicationApi.Application
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookDescription { get; set; }
        public string BookImage { get; set; }
        public string BookPdf { get; set; }
        public int Category { get; set; }
    }
}
