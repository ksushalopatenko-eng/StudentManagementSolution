using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentManagementWeb.Services;
using StudentManagementWeb.ViewModels;

namespace StudentManagementWeb.Pages.Enrollments;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;
    public IndexModel(ApiClient api) => _api = api;

    public List<SelectListItem> Students { get; set; } = new();
    public List<SelectListItem> Courses { get; set; } = new();
     public List<EnrollmentRowDto> EnrollmentRows { get; set; } = new();


    [BindProperty]
    public int StudentId { get; set; }

    [BindProperty]
    public int CourseId { get; set; }

    [TempData]
    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        await LoadListsAsync();
        EnrollmentRows = await _api.GetAllEnrollmentsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (StudentId <= 0 || CourseId <= 0)
        {
            Message = "Please select both student and course.";
            await LoadListsAsync();
            return Page();
        }

        try
        {
            await _api.EnrollAsync(new EnrollmentCreateDto(StudentId, CourseId));
            Message = "Enrollment created successfully.";
            return RedirectToPage("Index");
        }
        catch (HttpRequestException ex)
        {
            Message = $"API error: {ex.Message}";
            await LoadListsAsync();
            return Page();
        }
    }
    public async Task<IActionResult> OnPostDeleteAsync(int studentId, int courseId)
    {
        await _api.DeleteEnrollmentAsync(studentId, courseId);
        Message = "Enrollment removed.";
        return RedirectToPage("Index");
    }

    private async Task LoadListsAsync()
    {
        var students = await _api.GetStudentsAsync();
        var courses = await _api.GetCoursesAsync();

        Students = students
            .Select(s => new SelectListItem($"{s.Name} (Id: {s.Id})", s.Id.ToString()))
            .ToList();

        Courses = courses
            .Select(c => new SelectListItem($"{c.Title} (Id: {c.Id})", c.Id.ToString()))
            .ToList();
    }
   


}