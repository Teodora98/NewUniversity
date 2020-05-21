using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewUniversity.Data;
using NewUniversity.Models;
using NewUniversity.ViewModels;

namespace NewUniversity.Controllers
{
    //[Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {


        private readonly NewUniversityContext _context;
        public TeacherController(NewUniversityContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Teacher.ToList());
        }
        public async Task<IActionResult> Subjects(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjects = from c in _context.Course
                           .Include(e=>e.FirstTeacher)
                           .Include(e=>e.SecondTeacher)
                      where c.FirstTeacherId == id || c.SecondTeacherId == id
                      select c;

            if (subjects == null)
            {
                return NotFound();
            }
            return View(await subjects.ToListAsync());
        }
        public async Task<IActionResult> StudentSubject(int? id,int enrollmentYear)
        {
            if (id == null)
            {
                return NotFound();
            }
            /* var students = _context.Enrollment
                 .Include(e => e.Student)
                 .Include(e=>e.Course)
                 .Where(s => s.CourseId == id);
             return View(await students.ToListAsync());*/
            IQueryable<int?> year = _context.Enrollment.Select(m => m.Year).Distinct(); 
            var enrollments = _context.Enrollment
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(s => s.CourseId == id);
            if (enrollmentYear != 0)
            {
                enrollments = enrollments.Where(x => x.Year == enrollmentYear);
            }
            else
            {
                enrollments = enrollments.Where(x => x.Year == DateTime.Now.Year);
            }
            EnrollmentFilterViewModel enrolmentfilter = new EnrollmentFilterViewModel
            {
                SelectYear = new SelectList(await year.ToListAsync()),
                Enrollments = await enrollments.ToListAsync()
            };
            ViewData["CourseName"] = _context.Course.Where(e => e.Id == id).Select(e => e.Title).FirstOrDefault();
            return View(enrolmentfilter);
        }
        public async Task<IActionResult> Detal (int id)
        {
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
            public async Task<IActionResult> StudentEdit(int? id)
            {
            if (id == null)
            {
                return NotFound();
            }

            //var enrollment = await _context.Enrollment.FindAsync(id);
            var enrollment = await _context.Enrollment
                 .Include(e => e.Course)
                 .Include(e => e.Student)
                 .FirstOrDefaultAsync(e => e.Id == id);
            var broj = _context.Enrollment.Where(s => s.Id == id).Select(s => s.FinishDate).ToList();
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            //var tt = _context.Enrollment.Include(e => e.Student).Where(e => e.Id == id).AsQueryable();
            if (broj[0]==null) {
                return View(enrollment);
                
            }
            else
            {
                return RedirectToAction("Detal",new {id});
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentEdit(int id, [Bind("Id,Grade,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate,ProjectUrl,SeminalUrl,Year,CourseId,StudentId")] Enrollment enrollment)
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
                    return RedirectToAction("StudentSubject",new { id=enrollment.CourseId });
                
            } 
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }
     private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }

    }
}