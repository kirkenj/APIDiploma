using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace WebFront.RequestModels.Contracts
{
    public class MonthReportEditModel 
    {
        [Required]
        public int ContractID { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        [Range(0, int.MaxValue)]
        public double LectionsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double PracticalClassesTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double LaboratoryClassesTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double ConsultationsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double OtherTeachingClassesTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double CreditsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double ExamsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double CourseProjectsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double InterviewsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double TestsAndReferatsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double InternshipsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double DiplomasTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double DiplomasReviewsTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double SECTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double GraduatesManagementTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double GraduatesAcademicWorkTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double PlasticPosesDemonstrationTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public double TestingEscortTime { get; set; } = 0;
        [Range (0, int.MaxValue)]
        public double TimeSum =>
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
