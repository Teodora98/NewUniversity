using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewUniversity.Data;
using NewUniversity.Models;

namespace NewUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesApiController : ControllerBase
    {
        private readonly NewUniversityContext _context;

        public CoursesApiController(NewUniversityContext context)
        {
            _context = context;
        }

        // GET: api/CoursesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourse(int courseSemester, string Programme, string SearchString)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();
            IQueryable<int> semesterQ = _context.Course.OrderBy(m => m.Semester).Select(m => m.Semester).Distinct();
            IQueryable<string> programmeQ = _context.Course.OrderBy(m => m.Programme).Select(m => m.Programme).Distinct();
            if (courseSemester != 0)
            {
                courses = courses.Where(x => x.Semester == courseSemester);
            }
            if (!string.IsNullOrEmpty(Programme))
            {
                courses = courses.Where(x => x.Programme == Programme);
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                courses = courses.Where(s => s.Title.ToLower().Contains(SearchString.ToLower()));
            }
            return courses.ToList();
        }

        // GET: api/CoursesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var course = await _context.Course.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(course);
        }

        // PUT: api/CoursesApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CoursesApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // DELETE: api/CoursesApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Course>> DeleteCourse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }

        [HttpGet("{id}/GetStudentsInCourse")]
        public async Task<IActionResult> GetStudentsInCourse([FromRoute] int id,int enrollmentYear)
        {
            var courses = await _context.Course.FindAsync(id);
            var enrollments = _context.Enrollment.Where(m => m.CourseId == id);
            //IQueryable<int?> year = _context.Enrollment.Select(m => m.Year).Distinct();
            if (enrollmentYear != 0)
            {
                enrollments = enrollments.Where(x => x.Year == enrollmentYear);
            }
            else
            {
                enrollments = enrollments.Where(x => x.Year == DateTime.Now.Year);
            }
            List<Student> studentsList = new List<Student>();
            if (courses == null)
            { return NotFound(); }
            foreach (var student in enrollments)
            {
                Student newstudentlist = _context.Student.Where(m => m.Id == student.StudentId).FirstOrDefault();
                newstudentlist.Courses = null;
                studentsList.Add(newstudentlist);
            }
            return Ok(studentsList);
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
