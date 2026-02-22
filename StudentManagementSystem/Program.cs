using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;


// =======================
// Models
// =======================
class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }
}

class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
}

// связь Student <-> Course
class Enrollment
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
}
class UniversityData
{
    public List<Student> Students { get; set; } = new();
    public List<Course> Courses { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();
    public int NextStudentId { get; set; } = 1;
    public int NextCourseId { get; set; } = 1;
}
class CourseStatItem
{
    public string CourseTitle { get; set; } = "";
    public int Students { get; set; }
}

class UniversityStats
{
    public int StudentsCount { get; set; }
    public int CoursesCount { get; set; }
    public int EnrollmentsCount { get; set; }
    public double AverageAge { get; set; }

    public List<CourseStatItem> TopCourses { get; set; } = new();
    public List<Student> StudentsWithoutCourses { get; set; } = new();
}


// =======================
// Services (логика)
// =======================
class UniversityService
{
    private readonly List<Student> _students = new();
    private readonly List<Course> _courses = new();
    private readonly List<Enrollment> _enrollments = new();
    private const string DataFile = "data.json";


    private int _nextStudentId = 1;
    private int _nextCourseId = 1;

    public Student AddStudent(string name, int age)
    {
        var s = new Student
        {
            Id = _nextStudentId++,
            Name = name,
            Age = age
        };
        _students.Add(s);
        return s;
    }

    public Course AddCourse(string title)
    {
        var c = new Course
        {
            Id = _nextCourseId++,
            Title = title
        };
        _courses.Add(c);
        return c;
    }

    public List<Student> GetStudents() => _students;
    public List<Course> GetCourses() => _courses;

    public bool EnrollStudent(int studentId, int courseId, out string message)
    {
        var studentExists = _students.Any(s => s.Id == studentId);
        if (!studentExists)
        {
            message = "Student not found.";
            return false;
        }

        var courseExists = _courses.Any(c => c.Id == courseId);
        if (!courseExists)
        {
            message = "Course not found.";
            return false;
        }

        var already = _enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
        if (already)
        {
            message = "Student is already enrolled in this course.";
            return false;
        }

        _enrollments.Add(new Enrollment { StudentId = studentId, CourseId = courseId });
        message = "Enrolled successfully!";
        return true;
    }

    public List<Course> GetCoursesOfStudent(int studentId)
    {
        var courseIds = _enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId)
            .ToHashSet();

        return _courses.Where(c => courseIds.Contains(c.Id)).ToList();
    }

    public List<Student> GetStudentsOfCourse(int courseId)
    {
        var studentIds = _enrollments
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToHashSet();

        return _students.Where(s => studentIds.Contains(s.Id)).ToList();
    }
    public List<Student> FindStudentsByName(string query)
    {
        query = query.Trim().ToLower();

        return _students
            .Where(s => s.Name.ToLower().Contains(query))
            .ToList();
    }
    public bool DeleteStudent(int studentId, out string message)
    {
        var student = _students.FirstOrDefault(s => s.Id == studentId);
        if (student == null)
        {
            message = "Student not found.";
            return false;
        }

        _students.Remove(student);

        // удалить все записи (enrollments) этого студента
        _enrollments.RemoveAll(e => e.StudentId == studentId);

        message = "Student deleted (and enrollments removed).";
        return true;
    }
    public UniversityStats GetStatistics()
    {
        var stats = new UniversityStats
        {
            StudentsCount = _students.Count,
            CoursesCount = _courses.Count,
            EnrollmentsCount = _enrollments.Count,
            AverageAge = _students.Count == 0 ? 0 : _students.Average(s => s.Age)
        };

        // Top courses by enrollments
        var top = _enrollments
            .GroupBy(e => e.CourseId)
            .Select(g => new
            {
                CourseId = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(3)
            .ToList();

        foreach (var item in top)
        {
            var course = _courses.FirstOrDefault(c => c.Id == item.CourseId);
            if (course != null)
            {
                stats.TopCourses.Add(new CourseStatItem
                {
                    CourseTitle = course.Title,
                    Students = item.Count
                });
            }
        }

        // Students without courses
        var enrolledStudentIds = _enrollments
            .Select(e => e.StudentId)
            .ToHashSet();

        stats.StudentsWithoutCourses = _students
            .Where(s => !enrolledStudentIds.Contains(s.Id))
            .ToList();

        return stats;
    }

    public void Load()
    {
        if (!File.Exists(DataFile))
            return;

        try
        {
            string json = File.ReadAllText(DataFile);
            var data = JsonSerializer.Deserialize<UniversityData>(json);

            if (data == null) return;

            _students.Clear();
            _students.AddRange(data.Students);

            _courses.Clear();
            _courses.AddRange(data.Courses);

            _enrollments.Clear();
            _enrollments.AddRange(data.Enrollments);

            _nextStudentId = data.NextStudentId;
            _nextCourseId = data.NextCourseId;
        }
        catch
        {
            // если файл повреждён — просто не грузим
        }
    }

    public void Save()
    {
        var data = new UniversityData
        {
            Students = _students,
            Courses = _courses,
            Enrollments = _enrollments,
            NextStudentId = _nextStudentId,
            NextCourseId = _nextCourseId
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(data, options);
        File.WriteAllText(DataFile, json);
    }
    public bool EditStudent(int id, string newName, int newAge, out string message)
    {
        var student = _students.FirstOrDefault(s => s.Id == id);
        if (student == null)
        {
            message = "Student not found.";
            return false;
        }

        student.Name = newName;
        student.Age = newAge;

        message = "Student updated.";
        return true;
    }
    public bool EditCourse(int id, string newTitle, out string message)
    {
        var course = _courses.FirstOrDefault(c => c.Id == id);
        if (course == null)
        {
            message = "Course not found.";
            return false;
        }

        course.Title = newTitle;

        message = "Course updated.";
        return true;
    }


}

// =======================
// Program (UI/меню)
// =======================
class Program
{
    static UniversityService uni = new UniversityService();

    static void Main()
    {
        uni.Load();
        while (true)
        {
            ShowMenu();
            int choice = ReadInt("Choose option: ");

            Console.WriteLine();
            if (choice == 0)
            {
                uni.Save();
                break;
            }

            if (choice == 0) break;

            if (choice == 1) AddStudentFlow();
            else if (choice == 2) ShowStudentsFlow();
            else if (choice == 3) AddCourseFlow();
            else if (choice == 4) ShowCoursesFlow();
            else if (choice == 5) EnrollFlow();
            else if (choice == 6) ShowCoursesOfStudentFlow();
            else if (choice == 7) ShowStudentsOfCourseFlow();
            else if (choice == 8) FindStudentFlow();
            else if (choice == 9) DeleteStudentFlow();
            else if (choice == 10) ShowStatsFlow();
            else if (choice == 11) EditStudentFlow();
            else if (choice == 12) EditCourseFlow();
            else Console.WriteLine("Unknown option.");
            Console.WriteLine();
        }
        
    }

    static void ShowMenu()
    {
        Console.WriteLine("=== Student Management System ===");
        Console.WriteLine("1 - Add student");
        Console.WriteLine("2 - Show students");
        Console.WriteLine("3 - Add course");
        Console.WriteLine("4 - Show courses");
        Console.WriteLine("5 - Enroll student to course");
        Console.WriteLine("6 - Show courses of a student");
        Console.WriteLine("7 - Show students of a course");
        Console.WriteLine("8 - Find student by name");
        Console.WriteLine("9 - Delete student by Id");
        Console.WriteLine("10 - Show statistics");
        Console.WriteLine("11 - Edit student");
        Console.WriteLine("12 - Edit course");

        Console.WriteLine("0 - Exit");
    }

    static void AddStudentFlow()
    {
        string name = ReadString("Student name: ");
        int age = ReadInt("Student age: ");

        var s = uni.AddStudent(name, age);
        Console.WriteLine($"Added student: #{s.Id} {s.Name} ({s.Age})");
    }

    static void ShowStudentsFlow()
    {
        var students = uni.GetStudents();
        if (students.Count == 0)
        {
            Console.WriteLine("No students yet.");
            return;
        }

        foreach (var s in students)
            Console.WriteLine($"#{s.Id} {s.Name}, {s.Age}");
    }

    static void AddCourseFlow()
    {
        string title = ReadString("Course title: ");
        var c = uni.AddCourse(title);
        Console.WriteLine($"Added course: #{c.Id} {c.Title}");
    }

    static void ShowCoursesFlow()
    {
        var courses = uni.GetCourses();
        if (courses.Count == 0)
        {
            Console.WriteLine("No courses yet.");
            return;
        }

        foreach (var c in courses)
            Console.WriteLine($"#{c.Id} {c.Title}");
    }

    static void EnrollFlow()
    {
        int studentId = ReadInt("Student Id: ");
        int courseId = ReadInt("Course Id: ");

        bool ok = uni.EnrollStudent(studentId, courseId, out string msg);
        Console.WriteLine(msg);
    }

    static void ShowCoursesOfStudentFlow()
    {
        int studentId = ReadInt("Student Id: ");
        var courses = uni.GetCoursesOfStudent(studentId);

        if (courses.Count == 0)
        {
            Console.WriteLine("No courses for this student (or student not enrolled).");
            return;
        }

        Console.WriteLine("Courses:");
        foreach (var c in courses)
            Console.WriteLine($"#{c.Id} {c.Title}");
    }

    static void ShowStudentsOfCourseFlow()
    {
        int courseId = ReadInt("Course Id: ");
        var students = uni.GetStudentsOfCourse(courseId);

        if (students.Count == 0)
        {
            Console.WriteLine("No students in this course (or no enrollments).");
            return;
        }

        Console.WriteLine("Students:");
        foreach (var s in students)
            Console.WriteLine($"#{s.Id} {s.Name}, {s.Age}");
    }
    static void FindStudentFlow()
    {
        string query = ReadString("Enter name to search: ");

        var results = uni.FindStudentsByName(query);

        if (results.Count == 0)
        {
            Console.WriteLine("No students found.");
            return;
        }

        Console.WriteLine("Found:");
        foreach (var s in results)
            Console.WriteLine($"#{s.Id} {s.Name}, {s.Age}");
        }
        static void DeleteStudentFlow()
        {
        int studentId = ReadInt("Student Id to delete: ");

        bool ok = uni.DeleteStudent(studentId, out string msg);
        Console.WriteLine(msg);
    }
    static void ShowStatsFlow()
    {
        var stats = uni.GetStatistics();

        Console.WriteLine("=== Statistics ===");
        Console.WriteLine($"Students: {stats.StudentsCount}");
        Console.WriteLine($"Courses: {stats.CoursesCount}");
        Console.WriteLine($"Enrollments: {stats.EnrollmentsCount}");
        Console.WriteLine($"Average age: {stats.AverageAge:F2}");

        Console.WriteLine();
        Console.WriteLine("Top courses:");
        if (stats.TopCourses.Count == 0)
        {
            Console.WriteLine("No enrollments yet.");
        }
        else
        {
            foreach (var item in stats.TopCourses)
                Console.WriteLine($"{item.CourseTitle} - {item.Students} students");
        }

        Console.WriteLine();
        Console.WriteLine("Students without courses:");
        if (stats.StudentsWithoutCourses.Count == 0)
        {
            Console.WriteLine("None");
        }
        else
        {
            foreach (var s in stats.StudentsWithoutCourses)
                Console.WriteLine($"#{s.Id} {s.Name}, {s.Age}");
        }
    }
    static void EditStudentFlow()
    {
        int id = ReadInt("Student Id: ");
        string newName = ReadString("New name: ");
        int newAge = ReadInt("New age: ");

        bool ok = uni.EditStudent(id, newName, newAge, out string msg);
        Console.WriteLine(msg);
    }
    static void EditCourseFlow()
    {
        int id = ReadInt("Course Id: ");
        string newTitle = ReadString("New title: ");

        bool ok = uni.EditCourse(id, newTitle, out string msg);
        Console.WriteLine(msg);
    }



    // ===== Helpers for safe input =====
    static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";
            if (int.TryParse(input, out int value))
                return value;

            Console.WriteLine("Please enter a valid number.");
        }
    }

    static string ReadString(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine("Please enter a non-empty value.");
        }
    }
}
