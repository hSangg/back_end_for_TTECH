namespace tech_project_back_end.DTO
{
    public class UserDTO
    {
        public string? UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public string? Role { get; set; }
    }
}