 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewUniversity.Data;
using NewUniversity.Models;
using NewUniversity.ViewModels;

namespace NewUniversity.Controllers
{
    //[Authorize(Roles = "Student")]
    public class UserController : Controller
    {
        private readonly NewUniversityContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public UserController(NewUniversityContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View(_context.Student.ToList());
        }
        public async Task<IActionResult> Subjects(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subjects = _context.Enrollment.Include(e=>e.Course).Include(e=>e.Student).Where(e => e.StudentId == id);
            if (subjects == null)
            {
                return NotFound();
            }
            return View(await subjects.ToListAsync());
        }
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
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "FullName", enrollment.StudentId);
            EnrollmentViewModel vm = new EnrollmentViewModel
            {
                Id = enrollment.Id,
                Semester = enrollment.Semester,
                Year = enrollment.Year,
                Grade = enrollment.Grade,
                ProjectUrl = enrollment.ProjectUrl,
                SeminalPoints = enrollment.SeminalPoints,
                ProjectPoints = enrollment.ProjectPoints,
                AdditionalPoints = enrollment.AdditionalPoints,
                ExamPoints = enrollment.ExamPoints,
                FinishDate = enrollment.FinishDate,
                CourseId = enrollment.CourseId,
                StudentId = enrollment.StudentId
            };
            ViewData["StudentName"] = _context.Student.Where(s => s.Id == enrollment.StudentId).Select(s => s.FullName).FirstOrDefault();
            ViewData["CourseName"] = _context.Course.Where(s => s.Id == enrollment.CourseId).Select(s => s.Title).FirstOrDefault();
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  EnrollmentViewModel enrollment)
        {
            if (id != enrollment.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = UploadedFile(enrollment);

                    Enrollment enrollmentvm = new Enrollment
                    {
                        Id = enrollment.Id,
                        Semester = enrollment.Semester,
                        Year = enrollment.Year,
                        Grade = enrollment.Grade,
                        ProjectUrl = enrollment.ProjectUrl,
                        SeminalPoints = enrollment.SeminalPoints,
                        ProjectPoints = enrollment.ProjectPoints,
                        AdditionalPoints = enrollment.AdditionalPoints,
                        ExamPoints = enrollment.ExamPoints,
                        FinishDate = enrollment.FinishDate,
                        CourseId = enrollment.CourseId,
                        StudentId = enrollment.StudentId,
                        SeminalUrl=uniqueFileName
                    };
                    _context.Update(enrollmentvm);
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
        private string UploadedFile(EnrollmentViewModel model)
        {
            string uniqueFileName = null;

            if (model.SeminalUrl != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "projects");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.SeminalUrl.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.SeminalUrl.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollment.Any(e => e.Id == id);
        }
    }
}