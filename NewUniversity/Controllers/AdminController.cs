using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewUniversity.Data;
using NewUniversity.Models;
using NewUniversity.ViewModels;

namespace NewUniversity.Controllers
{
    public class AdminController : Controller
    {
        private readonly NewUniversityContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AdminController(NewUniversityContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> CourseIndex(int courseSemester, string courseProgramme, string searchString, string teacherString)
        {
            IQueryable<int> semesterQuery = _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<string> programmeQuery = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct();
            var courses = from m in _context.Course
                           .Include(m => m.FirstTeacher)
                           .Include(m => m.SecondTeacher)
                          select m;

            if (courseSemester != 0)
            {
                courses = courses.Where(x => x.Semester == courseSemester);
            }

            if (!string.IsNullOrEmpty(courseProgramme))
            {
                courses = courses.Where(x => x.Programme == courseProgramme);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(s => s.Title.Contains(searchString));
            }
            IEnumerable<Course> dataList = courses as IEnumerable<Course>;

            if (!string.IsNullOrEmpty(teacherString))
            {
                dataList = dataList.Where(s => s.FirstTeacher.FullName.ToLower().Contains(teacherString.ToLower()) || s.SecondTeacher.FullName.Contains(teacherString));
            }

            var courseViewModel = new CourseFilterViewModel
            {
                Semesters = new SelectList(await semesterQuery.ToListAsync()),
                Programmes = new SelectList(await programmeQuery.ToListAsync()),
                Courses = dataList.ToList()
            };
            return View(courseViewModel);
        }
        public async Task<IActionResult> CourseDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        public IActionResult CourseCreate()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FirstName");
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FirstName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourseCreate([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CourseIndex));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FirstName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FirstName", course.SecondTeacherId);
            return View(course);
        }
        public async Task<IActionResult> CourseEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = _context.Course.Where(m => m.Id == id).Include(m => m.Students).First();
            if (course == null)
            {
                return NotFound();
            }
            CourseStudentViewModel coursestudent = new CourseStudentViewModel
            {
                Course = course,
                StudentsList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = course.Students.Select(sa => sa.StudentId)
            };
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", course.SecondTeacherId);
            return View(coursestudent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourseEdit(int id, CourseStudentViewModel coursestudentviewmodel)
        {
            if (id != coursestudentviewmodel.Course.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coursestudentviewmodel.Course);
                    await _context.SaveChangesAsync();
                    IEnumerable<int> listStudents = coursestudentviewmodel.SelectedStudents;
                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);
                    IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                    IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                    foreach (int studentId in newStudents)
                        _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id });
                    await _context.SaveChangesAsync();

                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(coursestudentviewmodel.Course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CourseIndex));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", coursestudentviewmodel.Course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Teacher, "Id", "FullName", coursestudentviewmodel.Course.SecondTeacherId);
            return View(coursestudentviewmodel);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> CourseDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("CourseDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CourseDeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CourseIndex));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }

        public async Task<IActionResult> StudentIndex()
        {
            var students = await _context.Student.ToListAsync();
            return View(students);
        }
        public async Task<IActionResult> StudentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.Student
               .FirstOrDefaultAsync(m => m.Id == id);
            var course = _context.Enrollment
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.StudentId == id)
                .Select(s => s.Course.Title);
            string[] datalist = course.ToArray();
            ViewData["datalist"] = datalist;

            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        public IActionResult StudentCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentCreate(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                Student student = new Student
                {

                    Id = model.Id,
                    Index = model.Index,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EnrollmentDate = model.EnrollmentDate,
                    AcquiredCredits = model.AcquiredCredits,
                    CurrentSemestar = model.CurrentSemestar,
                    EducationLevel = model.EducationLevel,
                    UserProfileImage = uniqueFileName,
                };
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(StudentIndex));
            }
            return View();
        }
        private string UploadedFile(StudentViewModel model)
        {
            string uniqueFileName = null;

            if (model.UserProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" +Path.GetFileName(model.UserProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.UserProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public async Task<IActionResult> StudentEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.Student.FindAsync(id);
            
            if (student == null)
            {
                return NotFound();
            }
            StudentViewModel studentmodel = new StudentViewModel
            {

                Id = student.Id,
                Index = student.Index,
                FirstName = student.FirstName,
                LastName = student.LastName,
                EnrollmentDate = student.EnrollmentDate,
                AcquiredCredits = student.AcquiredCredits,
                CurrentSemestar = student.CurrentSemestar,
                EducationLevel = student.EducationLevel,
            };
            return View(studentmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentEdit(int id,StudentViewModel model)
        {
            if(id != model.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = UploadedFile(model);

                    Student student = new Student
                    {

                        Id = model.Id,
                        Index = model.Index,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EnrollmentDate = model.EnrollmentDate,
                        AcquiredCredits = model.AcquiredCredits,
                        CurrentSemestar = model.CurrentSemestar,
                        EducationLevel = model.EducationLevel,
                        UserProfileImage = uniqueFileName,
                    };
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(StudentIndex));
            }
            return View();
        }
        public async Task<IActionResult> StudentDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        [HttpPost, ActionName("StudentDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentDeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StudentIndex));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
        public async Task<IActionResult> TeacherIndex() 
        {
            /* var teachers = from m in _context.Teacher
                            select m;
             var t = from m in _context.Teacher
                     select m;
             IQueryable<string> rankQuery = _context.Teacher.OrderBy(m => m.AcademicRank).Select(m => m.AcademicRank).Distinct();
             IQueryable<string> degreeQuery = _context.Teacher.OrderBy(m => m.Degree).Select(m => m.Degree).Distinct();

             if (!string.IsNullOrEmpty(teacherAcademicRank))
             {
                 teachers = teachers.Where(x => x.AcademicRank == teacherAcademicRank);
             }

             if (!string.IsNullOrEmpty(teacherDegree))
             {
                 teachers = teachers.Where(x => x.Degree == teacherDegree);
             }

             IEnumerable<Teacher> datalist = teachers as IEnumerable<Teacher>;

             if (!string.IsNullOrEmpty(searchFullName))

             {
                 datalist = datalist.Where(s => s.FullName.ToLower().Contains(searchFullName.ToLower()));
             }

             var teacherViewModel = new TeacherFilterViewModel
             {
                 AcademicRanges = new SelectList(await rankQuery.ToListAsync()),
                 Degrees = new SelectList(await degreeQuery.ToListAsync()),
                 Teachers = datalist.ToList()
             };
             */
            var teachers = await _context.Teacher.ToListAsync();
            return View(teachers);
        }
        public async Task<IActionResult> TeacherDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        public IActionResult TeacherCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> TeacherCreate([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        public async Task<IActionResult> TeacherCreate(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFileTeacher(model);

                Teacher teacher = new Teacher
                {

                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Degree = model.Degree,
                    AcademicRank = model.AcademicRank,
                    OfficeNumber = model.OfficeNumber,
                    HireDate = model.HireDate,
                    TeacherProfileImage=uniqueFileName
                };
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TeacherIndex));
            }
            return View();
        }
        private string UploadedFileTeacher(TeacherViewModel model)
        {
            string uniqueFileName = null;

            if (model.TeacherProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.TeacherProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.TeacherProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public async Task<IActionResult> TeacherEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeacherEdit(int id, [Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(TeacherIndex));
            }
            return View(teacher);
        }
        public async Task<IActionResult> TeacherDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }
        [HttpPost, ActionName("TeacherDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeacherDeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TeacherIndex));
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }

        public async Task<IActionResult> EnrollmentIndex(string searchFullName, string searchCourse)
        {
            return View(await _context.Course.ToListAsync());
        }
        public async Task<IActionResult> EnrollmentDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .Where(e => e.CourseId == id)
                .OrderByDescending(e => e.Grade);

            if (enrollment == null)
            {
                return NotFound();
            }
            return View(await enrollment.ToListAsync());
        }
        public IActionResult EnrollmentCreate(int? id)
        {
            var courses = _context.Enrollment.Include(e => e.Student).Where(e => e.CourseId == id).FirstOrDefault();
            NewStudentEnrollmentsViewModel coursestudent = new NewStudentEnrollmentsViewModel
            {
                Enrollments = courses,
                StudentsList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = _context.Enrollment
                                    .Where(s => s.CourseId == id)
                                    .Include(m => m.Student).Select(sa => sa.StudentId)
            };
            return View(coursestudent);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollmentCreate(int id, NewStudentEnrollmentsViewModel newStudentEnrollmentsViewModel)
        {

                IEnumerable<int> listStudents = newStudentEnrollmentsViewModel.SelectedStudents;
                IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                foreach (int studentId in newStudents)
                    _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id, Year = newStudentEnrollmentsViewModel.Enrollments.Year, Semester = newStudentEnrollmentsViewModel.Enrollments.Semester });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EnrollmentIndex));
        }
        public async Task<IActionResult> EnrollmentEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollmentEdit(int id, [Bind("Id,Grade,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate,ProjectUrl,SeminalUrl,Year,CourseId,StudentId")] Enrollment enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EnrollmentIndex));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }
        public async Task<IActionResult> EnrollmentDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollment
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }
        [HttpPost, ActionName("EnrollmentDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollmentDeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EnrollmentIndex));
        }
        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}