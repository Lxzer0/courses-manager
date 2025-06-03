namespace CoursesManager.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required byte[] Image { get; set; }
        public required string Mime { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
    }
}
