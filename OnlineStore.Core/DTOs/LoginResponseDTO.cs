using System;
using System.Collections.Generic;

namespace OnlineStore.Core.DTOs;

public class LoginResponseDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public UserDTO User { get; set; } = new UserDTO();
}