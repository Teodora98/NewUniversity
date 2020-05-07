using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewUniversity.Data;
using NewUniversity.Models;
using NewUniversity.ViewModels;

namespace NewUniversity.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly NewUniversityContext _context;
        public EnrollmentsController(NewUniversityContext context)
        {
            _context = context;
        }
        // GET: Enrollments
        public async Task<IActionResult> Index (string searchFullName, string searchCourse)
        {
            return View(await _context.Course.ToListAsync());
        }

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Enrollments/Create
        public IActionResult Create(int? id,NewStudentEnrollmentsViewModel newStudentEnrollmentsViewModel)
        {
            /*ViewData["Id"] = id;
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName");
            return View();*/
            var courses = _context.Enrollment.Include(e => e.Student).Where(e => e.CourseId == id).First();
            NewStudentEnrollmentsViewModel coursestudent = new NewStudentEnrollmentsViewModel
            {
                Enrollments = courses,
                StudentsList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FullName"),
                SelectedStudents = _context.Enrollment
                                    .Where(s=>s.CourseId==id)
                                    .Include(m=>m.Student).Select(sa => sa.StudentId)
            };
            return View(coursestudent);
        }


        // POST: Enrollments/Create

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        /*public async Task<IActionResult> Create([Bind("CourseId,StudentId,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }*/
        public async Task<IActionResult> Create(int id, NewStudentEnrollmentsViewModel newStudentEnrollmentsViewModel)
        {
            if (ModelState.IsValid)
            {
                
                IEnumerable<int> listStudents = newStudentEnrollmentsViewModel.SelectedStudents;
                IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id).Select(s => s.StudentId);
                IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                foreach (int studentId in newStudents)
                    _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id, Year=newStudentEnrollmentsViewModel.Enrollments.Year, Semester=newStudentEnrollmentsViewModel.Enrollments.Semester});
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(newStudentEnrollmentsViewModel);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Semester,Year,Grade,SeminalUrl,ProjectUrl,ExamPoints,SeminalPoints,ProjectPoints,AdditionalPoints,FinishDate,CourseId,StudentId")] Enrollment enrollment)
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Title", enrollment.CourseId);
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FirstName", enrollment.StudentId);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollment.FindAsync(id);
            _context.Enrollment.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}
