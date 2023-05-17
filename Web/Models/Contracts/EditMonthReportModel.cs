using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Web.RequestModels.Contracts
{
    public class EditMonthReportModel 
    {
        [JsonIgnore]
        public int LinkingPartID { get; set; }
        [Required]
        public int ContractID { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Range(0, int.MaxValue)]
        public int LectionsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int PracticalClassesTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int LaboratoryClassesTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int ConsultationsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int OtherTeachingClassesTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int CreditsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int ExamsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int CourseProjectsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int InterviewsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int TestsAndReferatsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int InternshipsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int DiplomasTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int DiplomasReviewsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int SECTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int GraduatesManagementTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int GraduatesAcademicWorkTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int PlasticPosesDemonstrationTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int TestingEscortTime { get; set; } = 0;
        [Range (0, int.MaxValue)]
        public int TimeSum =>
            TestingEscortTime
            + PlasticPosesDemonstrationTime
            + GraduatesAcademicWorkTime
            + GraduatesManagementTime
            + SECTime
            + DiplomasReviewsTime
            + DiplomasTime
            + InternshipsTime
            + TestsAndReferatsTime
            + InterviewsTime
            + LectionsTime
            + PracticalClassesTime
            + LaboratoryClassesTime
            + ConsultationsTime
            + OtherTeachingClassesTime
            + CreditsTime
            + ExamsTime
            + CourseProjectsTime;
    }
}
