namespace Logic.RequestModels.Contracts
{
    public class ContractViewModelShorten
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DepartmentID { get; set; }
        public DateTime PeriodStart { get; set; } = DateTime.Now;
        public DateTime PeriodEnd { get; set; } = DateTime.Now.AddMonths(1);
        public bool IsConfirmed { get; set; } = false;
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
    }
}
