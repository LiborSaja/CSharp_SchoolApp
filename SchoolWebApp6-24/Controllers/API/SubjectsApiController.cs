using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp6_24.DTO;
using SchoolWebApp6_24.Services;

namespace SchoolWebApp6_24.Controllers.API {
    [Route("API/Subjects")]
    [ApiController]
    public class SubjectsApiController : ControllerBase {
        private SubjectService _subjectService;

        public SubjectsApiController(SubjectService subjectService) {
            _subjectService = subjectService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SubjectDTO>> Index() {
            var allSubjects = _subjectService.GetSubjects();
            return Ok(allSubjects);
        }
    }
}
