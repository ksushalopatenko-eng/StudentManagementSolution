using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;
    public IndexModel(ApiClient api) => _api = api;

    public List<CourseDto> Courses { get; set; } = new();

    public async Task OnGetAsync()
    {
        Courses = await _api.GetCoursesAsync();
    }
}