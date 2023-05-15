using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(Contract)+"s")]
    public class Contract : IConfirmableByAdminObject, IIdObject<int>
    {
        public int ID { get; set; }
        public User User { get; set; } = null!;
        public int UserID { get; set; }
        public int DepartmentID { get; set; }
        public int ContractTypeID { get; set; }
        public string ContractIdentifier { get; set; } = null!;
        public Department? Department { get; set; } = null!;
        public ContractType? ContractType { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public bool IsConfirmed => ConfirmedByUserID != null;
        public int? ParentContractID { get; set; }
        public Contract? ParentContract { get; set; } = null;
        public int? ChildContractID { get; set; }
        public Contract? ChildContract { get; set; } = null;
        public int? ConfirmedByUserID { get; set; }
        public User? ConfirmedByUser { get; set; }
        public int? LinkingPartID { get; set; }
        public ContractLinkingPart LinkingPart { get; set; } = null!;
        public DateTime AssignmentDate { get; set; }

        #region TablePart
        public double LectionsMaxTime { get; set; } = 0;
        public double PracticalClassesMaxTime { get; set; } = 0;
        public double LaboratoryClassesMaxTime { get; set; } = 0;
        public double ConsultationsMaxTime { get; set; } = 0;
        public double OtherTeachingClassesMaxTime { get; set; } = 0;
        public double CreditsMaxTime { get; set; } = 0;
        public double ExamsMaxTime { get; set; } = 0;
        public double CourseProjectsMaxTime { get; set; } = 0;
        public double InterviewsMaxTime { get; set; } = 0;
        public double TestsAndReferatsMaxTime { get; set; } = 0;
        public double InternshipsMaxTime { get; set; } = 0;
        public double DiplomasMaxTime { get; set; } = 0;
        public double DiplomasReviewsMaxTime { get; set; } = 0;
        public double SECMaxTime { get; set; } = 0;
        public double GraduatesManagementMaxTime { get; set; } = 0;
        public double GraduatesAcademicWorkMaxTime { get; set; } = 0;
        public double PlasticPosesDemonstrationMaxTime { get; set; } = 0;
        public double TestingEscortMaxTime { get; set; } = 0;

        public double TimeSum =>
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
        #endregion
    }
}