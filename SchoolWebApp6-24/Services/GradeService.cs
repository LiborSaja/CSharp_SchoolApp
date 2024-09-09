using Microsoft.EntityFrameworkCore;
using SchoolWebApp6_24.DTO;
using SchoolWebApp6_24.Models;
using SchoolWebApp6_24.ViewModels;

namespace SchoolWebApp6_24.Services {
    public class GradeService {
        private ApplicationDbContext _context;

        public GradeService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<GradesDropdownsViewModel> GetGradesDropdownsData() {
            var gradesDropdownsData = new GradesDropdownsViewModel() {
                Students = await _context.Students.OrderBy(student => student.LastName).ToListAsync(),      //seřazení podle příjmení
                Subjects = await _context.Subjects.OrderBy(Subject => Subject.Name).ToListAsync()           //seřazení podle jména předmětu
            };
            return gradesDropdownsData;
        }

        internal async Task CreateAsync(GradeDTO gradeDTO) {
            Grade gradeToInsert = await DtoToModel(gradeDTO);
            await _context.Grades.AddAsync(gradeToInsert);
            await _context.SaveChangesAsync();
        }

        private async Task<Grade> DtoToModel(GradeDTO gradeDTO) {
            return new Grade() {
                Date = DateTime.Today,
                Mark = gradeDTO.Mark,
                Topic = gradeDTO.Topic,
                Student = await _context.Students.FirstOrDefaultAsync(student => student.Id == gradeDTO.StudentId),
                Subject = await _context.Subjects.FirstOrDefaultAsync(subject => subject.Id == gradeDTO.SubjectId),
                Id = gradeDTO.Id
            };
        }

        public async Task<IEnumerable<GradesViewModel>> GetAllVMsAsync() {
            List<Grade> grades = await _context.Grades.Include(gr=>gr.Student).Include(sub=>sub.Subject).ToListAsync();
            List<GradesViewModel> gradeVMs = new List<GradesViewModel>();
            foreach (Grade grade in grades) {
                gradeVMs.Add(new GradesViewModel {
                    Date = grade.Date,
                    Id = grade.Id,
                    Mark = grade.Mark,
                    StudentName = $"{grade.Student.LastName} {grade.Student.FirstName}",
                    SubjectName = grade.Subject.Name,
                    Topic = grade.Topic,
                });
            }
            return gradeVMs;
        }

        internal async Task<Grade> GetByIdAsync(int id) {
            return await _context.Grades.Include(gr=>gr.Student).Include(gr=>gr.Subject).FirstOrDefaultAsync(grade => grade.Id == id);
        }

        internal GradeDTO ModelToDto(Grade grade) {
            return new GradeDTO {
                Id = grade.Id,
                Date = grade.Date,
                Mark = grade.Mark,
                StudentId = grade.Student.Id,
                SubjectId = grade.Subject.Id,
                Topic = grade.Topic
            };
        }

        internal async Task UpdateAsync(int id, GradeDTO gradeDTO) {
            Grade updatedGrade = await DtoToModel(gradeDTO);
            _context.Grades.Update(updatedGrade);
            _context.SaveChangesAsync();           
        }

        internal async Task DeleteAsync(int id) {
            var gradeToDelete = await _context.Grades.FirstOrDefaultAsync(g=>g.Id==id);
            _context.Grades.Remove(gradeToDelete);
            _context.SaveChangesAsync();
        }

        public async Task<int> GetGradesCountAsync() {
            return await _context.Grades.CountAsync();
        }
    }
}
