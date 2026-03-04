using Npgsql.Internal.Postgres;

namespace cetuspro0203.Entities
{
    public class LoginRequest
    {
        public int Id { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public enum Role { Admin }
    }
}
