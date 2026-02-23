using System.Net.Http.Json;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _factory;
    public ApiClient(IHttpClientFactory factory) => _factory = factory;
    private HttpClient Client => _factory.CreateClient("StudentApi");

    // Students
    public async Task<List<StudentDto>> GetStudentsAsync()
        => await Client.GetFromJsonAsync<List<StudentDto>>("students") ?? new();

    public async Task<StudentDto?> GetStudentAsync(int id)
        => await Client.GetFromJsonAsync<StudentDto>($"students/{id}");

    public async Task CreateStudentAsync(StudentCreateDto dto)
        => (await Client.PostAsJsonAsync("students", dto)).EnsureSuccessStatusCode();

    public async Task UpdateStudentAsync(int id, StudentUpdateDto dto)
        => (await Client.PutAsJsonAsync($"students/{id}", dto)).EnsureSuccessStatusCode();

    public async Task DeleteStudentAsync(int id)
        => (await Client.DeleteAsync($"students/{id}")).EnsureSuccessStatusCode();

    // Courses
    public async Task<List<CourseDto>> GetCoursesAsync()
        => await Client.GetFromJsonAsync<List<CourseDto>>("courses") ?? new();

    public async Task<CourseDto?> GetCourseAsync(int id)
        => await Client.GetFromJsonAsync<CourseDto>($"courses/{id}");

    public async Task CreateCourseAsync(CourseCreateDto dto)
        => (await Client.PostAsJsonAsync("courses", dto)).EnsureSuccessStatusCode();

    public async Task UpdateCourseAsync(int id, CourseUpdateDto dto)
        => (await Client.PutAsJsonAsync($"courses/{id}", dto)).EnsureSuccessStatusCode();

    public async Task DeleteCourseAsync(int id)
        => (await Client.DeleteAsync($"courses/{id}")).EnsureSuccessStatusCode();

    // Enrollments
    public async Task EnrollAsync(EnrollmentCreateDto dto)
        => (await Client.PostAsJsonAsync("enrollments", dto)).EnsureSuccessStatusCode();

    public async Task<List<CourseDto>> GetStudentCoursesAsync(int studentId)
        => await Client.GetFromJsonAsync<List<CourseDto>>($"enrollments/students/{studentId}/courses") ?? new();

    public async Task<List<StudentDto>> GetCourseStudentsAsync(int courseId)
        => await Client.GetFromJsonAsync<List<StudentDto>>($"enrollments/courses/{courseId}/students") ?? new();
    public async Task<List<EnrollmentRowDto>> GetAllEnrollmentsAsync()
    => await Client.GetFromJsonAsync<List<EnrollmentRowDto>>("enrollments") ?? new();

    public async Task DeleteEnrollmentAsync(int studentId, int courseId)
    => (await Client.DeleteAsync($"enrollments/students/{studentId}/courses/{courseId}"))
        .EnsureSuccessStatusCode();
}