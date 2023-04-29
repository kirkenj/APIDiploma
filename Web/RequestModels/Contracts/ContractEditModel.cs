using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace Web.RequestModels.Contracts
{
    public class ContractEditModel : IValidatableObject
    {
        [Required]
        public int ContractID { get; set; }
        [Required]
        public int? UserId { get; set; } = -1;
        [Required]
        public int DepartmentID { get; set; }
        public int? ParentContractID { get; set; }
        public DateTime PeriodStart { get; set; } = DateTime.Now;
        public DateTime PeriodEnd { get; set; } = DateTime.Now.AddMonths(1);
        [Range(0, int.MaxValue)]
        public int LectionsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int PracticalClassesMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int LaboratoryClassesMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int ConsultationsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int OtherTeachingClassesMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int CreditsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int ExamsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int CourseProjectsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int InterviewsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int TestsAndReferatsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int InternshipsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int DiplomasMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int DiplomasReviewsMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int SECMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int GraduatesManagementMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int GraduatesAcademicWorkMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int PlasticPosesDemonstrationMaxTime { get; set; } = 0;
        [Range(0, int.MaxValue)]
        public int TestingEscortMaxTime { get; set; } = 0;
        public int TimeSum =>
                TestingEscortMaxTime
                + PlasticPosesDemonstrationMaxTime
                + GraduatesAcademicWorkMaxTime
                + GraduatesManagementMaxTime
                + SECMaxTime
                + DiplomasReviewsMaxTime
                + DiplomasMaxTime
                + InternshipsMaxTime
                + TestsAndReferatsMaxTime
                + InterviewsMaxTime
                + LectionsMaxTime
                + PracticalClassesMaxTime
                + LaboratoryClassesMaxTime
                + ConsultationsMaxTime
                + OtherTeachingClassesMaxTime
                + CreditsMaxTime
                + ExamsMaxTime
                + CourseProjectsMaxTime;
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PeriodStart > PeriodEnd) yield return new ValidationResult($"PeriodStart > PeriodEnd");
            if (TimeSum == 0) yield return new ValidationResult("TimeSum = 0");
        }
    }
}
