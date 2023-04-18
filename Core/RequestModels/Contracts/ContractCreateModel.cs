using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Logic.RequestModels.Contracts
{
    public class ContractCreateModel : IValidatableObject
    {
        [JsonIgnore]
        public int? UserId { get; set; } = -1;
        public int DepartmentID{ get; set; }
        public DateTime PeriodStart { get; set; } = DateTime.Now;
        public DateTime PeriodEnd { get; set; } = DateTime.Now.AddMonths(1);
        public int LectionsMaxTime { get; set; } = 0;
        public int PracticalClassesMaxTime { get; set; } = 0;
        public int LaboratoryClassesMaxTime { get; set; } = 0;
        public int ConsultationsMaxTime { get; set; } = 0;
        public int OtherTeachingClassesMaxTime { get; set; } = 0;
        public int CreditsMaxTime { get; set; } = 0;
        public int ExamsMaxTime { get; set; } = 0;
        public int CourseProjectsMaxTime { get; set; } = 0;
        public int InterviewsMaxTime { get; set; } = 0;
        public int TestsAndReferatsMaxTime { get; set; } = 0;
        public int InternshipsMaxTime { get; set; } = 0;
        public int DiplomasMaxTime { get; set; } = 0;
        public int DiplomasReviewsMaxTime { get; set; } = 0;
        public int SECMaxTime { get; set; } = 0;
        public int GraduatesManagementMaxTime { get; set; } = 0;
        public int GraduatesAcademicWorkMaxTime { get; set; } = 0;
        public int PlasticPosesDemonstrationMaxTime { get; set; } = 0;
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserId is null)
            {
                yield return new ValidationResult($"User ID is not defined");
            }

            if (PeriodStart > PeriodEnd)
            {
                yield return new ValidationResult($"PeriodStart > PeriodEnd");
            }

            if (TimeSum < 0)
            {
                yield return new ValidationResult($"{nameof(TimeSum)} < 0");
            }

            if (LectionsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(LectionsMaxTime)} < 0");
            }

            if (PracticalClassesMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(PracticalClassesMaxTime)} < 0");
            }

            if (LaboratoryClassesMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(LaboratoryClassesMaxTime)} < 0");
            }

            if (ConsultationsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(ConsultationsMaxTime)} < 0");
            }

            if (OtherTeachingClassesMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(OtherTeachingClassesMaxTime)} < 0");
            }

            if (CreditsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(CreditsMaxTime)} < 0");
            }

            if (ExamsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(ExamsMaxTime)} < 0");
            }

            if (CourseProjectsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(CourseProjectsMaxTime)} < 0");
            }

            if (InterviewsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(InterviewsMaxTime)} < 0");
            }

            if (TestsAndReferatsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(TestsAndReferatsMaxTime)} < 0");
            }

            if (InternshipsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(InternshipsMaxTime)} < 0");
            }

            if (DiplomasMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(DiplomasMaxTime)} < 0");
            }

            if (DiplomasReviewsMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(DiplomasReviewsMaxTime)} < 0");
            }

            if (SECMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(SECMaxTime)} < 0");
            }

            if (GraduatesManagementMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(GraduatesManagementMaxTime)} < 0");
            }

            if (GraduatesAcademicWorkMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(GraduatesAcademicWorkMaxTime)} < 0");
            }

            if (PlasticPosesDemonstrationMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(PlasticPosesDemonstrationMaxTime)} < 0");
            }

            if (TestingEscortMaxTime < 0)
            {
                yield return new ValidationResult($"{nameof(TestingEscortMaxTime)} < 0");
            }
        }
    }
}
