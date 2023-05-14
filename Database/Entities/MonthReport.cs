using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(MonthReport) + "s")]
    public class MonthReport : IValidatableObject
    {
        public int ContractID { get; set; }
        public Contract Contract { get; set; } = null!;
        public int Month { get; set; }
        public int Year { get; set; }
        public double LectionsTime { get; set; } = 0;
        public double PracticalClassesTime { get; set; } = 0;
        public double LaboratoryClassesTime { get; set; } = 0;
        public double ConsultationsTime { get; set; } = 0;
        public double OtherTeachingClassesTime { get; set; } = 0;
        public double CreditsTime { get; set; } = 0;
        public double ExamsTime { get; set; } = 0;
        public double CourseProjectsTime { get; set; } = 0;
        public double InterviewsTime { get; set; } = 0;
        public double TestsAndReferatsTime { get; set; } = 0;
        public double InternshipsTime { get; set; } = 0;
        public double DiplomasTime { get; set; } = 0;
        public double DiplomasReviewsTime { get; set; } = 0;
        public double SECTime { get; set; } = 0;
        public double GraduatesManagementTime { get; set; } = 0;
        public double GraduatesAcademicWorkTime { get; set; } = 0;
        public double PlasticPosesDemonstrationTime { get; set; } = 0;
        public double TestingEscortTime { get; set; } = 0;
        public int? BlockedByUserID { get; set; } = null;
        public User BlockedByUser { get; set; } = null!;
        public bool IsBlocked => BlockedByUserID != null;
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TimeSum < 0)
            {
                yield return new ValidationResult($"{nameof(TimeSum)} < 0");
            }

            if (TimeSum > 72)
            {
                yield return new ValidationResult($"{nameof(TimeSum)} > 72");
            }

            if (LectionsTime < 0)
            {
                yield return new ValidationResult($"{nameof(LectionsTime)} < 0");
            }

            if (PracticalClassesTime < 0)
            {
                yield return new ValidationResult($"{nameof(PracticalClassesTime)} < 0");
            }

            if (LaboratoryClassesTime < 0)
            {
                yield return new ValidationResult($"{nameof(LaboratoryClassesTime)} < 0");
            }

            if (ConsultationsTime < 0)
            {
                yield return new ValidationResult($"{nameof(ConsultationsTime)} < 0");
            }

            if (OtherTeachingClassesTime < 0)
            {
                yield return new ValidationResult($"{nameof(OtherTeachingClassesTime)} < 0");
            }   

            if (CreditsTime < 0)
            {   
                yield return new ValidationResult($"{nameof(CreditsTime)} < 0");
            }

            if (ExamsTime < 0)
            {
                yield return new ValidationResult($"{nameof(ExamsTime)} < 0");
            }

            if (CourseProjectsTime < 0)
            {
                yield return new ValidationResult($"{nameof(CourseProjectsTime)} < 0");
            }

            if (InterviewsTime < 0)
            {
                yield return new ValidationResult($"{nameof(InterviewsTime)} < 0");
            }

            if (TestsAndReferatsTime < 0)
            {
                yield return new ValidationResult($"{nameof(TestsAndReferatsTime)} < 0");
            }

            if (InternshipsTime < 0)
            {
                yield return new ValidationResult($"{nameof(InternshipsTime)} < 0");
            }

            if (DiplomasTime < 0)
            {
                yield return new ValidationResult($"{nameof(DiplomasTime)} < 0");
            }

            if (DiplomasReviewsTime < 0)
            {
                yield return new ValidationResult($"{nameof(DiplomasReviewsTime)} < 0");
            }

            if (SECTime < 0)
            {
                yield return new ValidationResult($"{nameof(SECTime)} < 0");
            }

            if (GraduatesManagementTime < 0)
            {
                yield return new ValidationResult($"{nameof(GraduatesManagementTime)} < 0");
            }

            if (GraduatesAcademicWorkTime < 0)
            {
                yield return new ValidationResult($"{nameof(GraduatesAcademicWorkTime)} < 0");
            }

            if (PlasticPosesDemonstrationTime < 0)
            {
                yield return new ValidationResult($"{nameof(PlasticPosesDemonstrationTime)} < 0");
            }

            if (TestingEscortTime < 0)
            {
                yield return new ValidationResult($"{nameof(TestingEscortTime)} < 0");
            }
        }
    }
}
