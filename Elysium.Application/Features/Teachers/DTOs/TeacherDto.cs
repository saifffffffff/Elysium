namespace Elysium.Application.Features.Teachers.DTOs;

public record TeacherDto(int id, string username, string firstname, string lastname, DateOnly birthDate);