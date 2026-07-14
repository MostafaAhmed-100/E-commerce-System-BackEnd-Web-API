namespace WebApplication1.DTOS.Response_DTOs
{
    public class WishlistDto
    {
        public int Id { get; set; }

        public string ListName { get; set; }

        public DateTime CreatedAt { get; set; }

        public int ItemsCount { get; set; }
    }
}
