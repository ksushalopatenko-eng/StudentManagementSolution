using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Pages.Enrollments;

public class StudentCoursesModel : PageModel
{
    private readonly ApiClient _api;
    public StudentCoursesModel(ApiClient api) => _api = api;

    [BindProperty(SupportsGet = true)]
    public int StudentId { get; set; }

    public StudentDto? Student { get; set; }
    public List<CourseDto> Courses { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Student = await _api.GetStudentAsync(StudentId);
        if (Student is null) return NotFound();

        Courses = await _api.GetStudentCoursesAsync(StudentId);
        return Page();
    }
}