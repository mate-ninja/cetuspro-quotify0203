namespace cetuspro0203.Entities
{
    public class LoginRequest
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
