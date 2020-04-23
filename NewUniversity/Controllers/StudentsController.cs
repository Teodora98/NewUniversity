using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewUniversity.Data;
using NewUniversity.Models;
using NewUniversity.ViewModels;

namespace NewUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly NewUniversityContext _context;

        public StudentsController(NewUniversityContext context)
        {
            _context = context;
        }
        // GET: Students
        public async Task<IActionResult> Index(string studentIndex, string searchFullName, string searchCourse)
        {
            IQueryable<string> indexQuery = _context.Student.OrderBy(m => m.Index).Select(m => m.Index).Distinct();
            IQueryable<Student> students = _context.Student.AsQueryable()
                        .Include(s => s.Courses)
                        .ThenInclude(s => s.Course);
            var enrollments = _context.Enrollment
                          .Include(e => e.Course)
                          .Include(e => e.Student)
                          .Where(s => s.Course.Title.Contains(searchCourse));
           
            IEnumerable<int> enrollmentsID = enrollments.Select(e => e.StudentId).Distinct();

            if (!string.IsNullOrEmpty(searchCourse))
            {
                students = students.Where(s => enrollmentsID.Contains(s.Id));
            }

            IEnumerable<Student> dataList = students as IEnumerable<Student>;

            if (!string.IsNullOrEmpty(studentIndex))
            {
                dataList = dataList.Where(x => x.Index == studentIndex);
            }

            if (!string.IsNullOrEmpty(searchFullName))
            {
                dataList = dataList.Where(s => s.FullName.ToLower().Contains(searchFullName.ToLower())).ToList();
            }

            var studentViewModel = new StudentFilterViewModel
            {
                Students = dataList.ToList()
            };
            return View(studentViewModel);
        }
        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
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
            //IEnumerable<string[]> datalist1 = course as IEnumerable<string[]>;
            ViewData["datalist"] = datalist;
            
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Index,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Index,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
