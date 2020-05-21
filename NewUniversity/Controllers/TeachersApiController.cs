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
    public class TeachersApiController : ControllerBase
    {
        private readonly NewUniversityContext _context;

        public TeachersApiController(NewUniversityContext context)
        {
            _context = context;
        }

        // GET: api/TeachersApi
        [HttpGet]
            public async Task<ActionResult<IEnumerable<Teacher>>> GetTeacher(string AcademicRank, string Degree, string SearchString)
            {

                IEnumerable<Teacher> teachers = _context.Teacher;
                IQueryable<string> RankQ = _context.Teacher.OrderBy(m => m.AcademicRank!=null).Select(m => m.AcademicRank).Distinct();
                IQueryable<string> DegreeQ = _context.Teacher.OrderBy(m => m.Degree != null).Select(m => m.Degree).Distinct();

                if (!string.IsNullOrEmpty(AcademicRank))
                {
                    teachers = teachers.Where(x => x.AcademicRank == AcademicRank);
                }
                if (!string.IsNullOrEmpty(Degree))
                {
                    teachers = teachers.Where(x => x.Degree == Degree);
                }

                if (!string.IsNullOrEmpty(SearchString))
                {
                    teachers = teachers.Where(s => (s.FirstName + " " + s.LastName).ToLower().Contains(SearchString.ToLower())).ToList();
 
                }

                return teachers.ToList();
            }

        // GET: api/TeachersApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetTeacher(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }

        // PUT: api/TeachersApi/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest();
            }

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
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

        // POST: api/TeachersApi
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacher)
        {
            _context.Teacher.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacher", new { id = teacher.Id }, teacher);
        }

        // DELETE: api/TeachersApi/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Teacher>> DeleteTeacher(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();

            return teacher;
        }
        [HttpGet("{id}/GetCoursesForTeacher")]
        public async Task<IActionResult> GetCoursesForTeacher([FromRoute] int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null) { return NotFound(); }

            var courses = _context.Course.Where(m => m.FirstTeacherId == id || m.SecondTeacherId == id);
            foreach (var course in courses)
            {
                course.Students = null;
                course.FirstTeacher = null;
                course.SecondTeacher = null;
            }
            return Ok(courses);
        }
        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }
    }
}
