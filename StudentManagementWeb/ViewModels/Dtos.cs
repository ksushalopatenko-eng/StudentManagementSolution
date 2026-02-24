namespace StudentManagementWeb.ViewModels;

public record StudentDto(int Id, string Name, int Age);
public record StudentCreateDto(string Name, int Age);
public record StudentUpdateDto(string Name, int Age);

public record CourseDto(int Id, string Title);
public record CourseCreateDto(string Title);
public record CourseUpdateDto(string Title);
public record StatsDto(int Students, int Courses, int Enrollments);

public record EnrollmentCreateDto(int StudentId, int CourseId);
public record EnrollmentRowDto(
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseTitle
);