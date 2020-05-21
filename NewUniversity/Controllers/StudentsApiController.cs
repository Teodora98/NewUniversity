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
    public class StudentsApiController : ControllerBase
    {
        private readonly NewUniversityContext _context;

        public StudentsApiController(NewUniversityContext context)
        {
            _context = context;
        }

        // GET: api/StudentsApi
       
        [HttpGet]
        public List<Student> GetStudent(string Index, string searchString)
        {
            IQueryable<Student> students = _context.Student.AsQueryable();
            IQueryable<string> indexQuery = _context.Student.OrderBy(m => m.Index).Select(m => m.Index).Distinct();

            if (!string.IsNullOrEmpty(Index))
            {
                students = students.Where(x => x.Index == Index);
            }

            IEnumerable<Student> dataList = students as IEnumerable<Student>;

            if (!string.IsNullOrEmpty(searchString))
            {
                dataList = dataList.ToList().Where(s => s.FullName.ToLower().Contains(searchString.ToLower()));
            }

            return dataList.ToList();
        }

        // GET: api/StudentsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var student = await _context.Student.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/StudentsApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/StudentsApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        // DELETE: api/StudentsApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> DeleteStudent(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return student;
        }

        [HttpGet("{id}/GetCoursesForStudent")]
        public async Task<IActionResult> GetCoursesForStudent([FromRoute] int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null) { return NotFound(); }

            var enrollments = _context.Enrollment.Where(m => m.StudentId == id).ToList();
            List<Course> courses = new List<Course>();
            foreach (var course in enrollments)
            {
                Course newcourseList = _context.Course.Where(m => m.Id == course.CourseId).FirstOrDefault();
                newcourseList.Students = null; 
                courses.Add(newcourseList);
            }
            return Ok(courses);  
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}
