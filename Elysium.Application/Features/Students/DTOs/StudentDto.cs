namespace Elysium.Application.Features.Students.DTOs;

public record StudentDto(int id, string username, string firstname, string lastname, DateOnly birthDate);