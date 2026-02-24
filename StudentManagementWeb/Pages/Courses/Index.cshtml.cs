// using Microsoft.AspNetCore.Mvc.RazorPages;
// using StudentManagementWeb.Services;
// using StudentManagementWeb.ViewModels;

// namespace StudentManagementWeb.Pages.Courses;

// public class IndexModel : PageModel
// {
//     private readonly ApiClient _api;
//     public IndexModel(ApiClient api) => _api = api;

//     public List<CourseDto> Courses { get; set; } = new();

//     public async Task OnGetAsync()
//     {
//         Courses = await _api.GetCoursesAsync();
//     }
// }
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;
using System.Linq;

namespace StudentManagementWeb.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;
    public IndexModel(ApiClient api) => _api = api;

    public List<CourseDto> AllCourses { get; set; } = new();
    public List<CourseDto> SearchResults { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public bool HasSearch => !string.IsNullOrWhiteSpace(Search);
    public bool HasResults => SearchResults.Count > 0;

    public async Task OnGetAsync()
    {
        var all = await _api.GetCoursesAsync();
        AllCourses = all;

        if (HasSearch)
        {
            var term = Search!.Trim();

            SearchResults = all
                .Where(c => c.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _api.DeleteCourseAsync(id);
        return RedirectToPage(new { Search });
    }
}