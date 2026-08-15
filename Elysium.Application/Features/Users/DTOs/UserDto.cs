using Elysium.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Application.Features.Users.DTOs;

public record UserDto(int id, string username, string firstname, string lastname, DateOnly birthDate, UserRole role);

