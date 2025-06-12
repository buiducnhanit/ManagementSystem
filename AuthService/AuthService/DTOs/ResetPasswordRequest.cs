﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class ResetPasswordRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Token {  get; set; } = string.Empty;

        [Required]
        [PasswordPropertyText]
        public string NewPassword { get; set; } = string.Empty;
    }
}
