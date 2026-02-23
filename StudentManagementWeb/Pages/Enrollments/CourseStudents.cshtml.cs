using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Pages.Enrollments;

public class CourseStudentsModel : PageModel
{
    private readonly ApiClient _api;
    public CourseStudentsModel(ApiClient api) => _api = api;

    [BindProperty(SupportsGet = true)]
    public int CourseId { get; set; }

    public CourseDto? Course { get; set; }
    public List<StudentDto> Students { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Course = await _api.GetCourseAsync(CourseId);
        if (Course is null) return NotFound();

        Students = await _api.GetCourseStudentsAsync(CourseId);
        return Page();
    }
}