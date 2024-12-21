namespace EventManagementSystem.Utilities
{
    public interface IPasswordHasher
    {
        string HashPassword(string password, string salt);
        string GetSalt();
        bool ValidatePassword(string password, string salt, string hash);
    }
}
