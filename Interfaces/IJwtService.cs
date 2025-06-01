namespace CoursesManager.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(Guid uuid);
    }
}
