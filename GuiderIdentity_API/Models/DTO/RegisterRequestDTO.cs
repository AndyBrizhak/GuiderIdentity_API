﻿namespace GuiderIdentity_API.Models.DTO
{
    public class RegisterRequestDTO
    {
        public required string UserName { get; set; }
        public string? Name { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }

    }
}
