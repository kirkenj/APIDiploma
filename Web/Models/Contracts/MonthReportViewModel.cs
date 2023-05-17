namespace Web.RequestModels.Contracts
{
    public class MonthReportViewModel
    {
        public int ContractID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsBlocked { get; set; }
        public int LinkingPartID { get; set; }
        public int LectionsTime { get; set; } = 0;
        public int PracticalClassesTime { get; set; } = 0;
        public int LaboratoryClassesTime { get; set; } = 0;
        public int ConsultationsTime { get; set; } = 0;
        public int OtherTeachingClassesTime { get; set; } = 0;
        public int CreditsTime { get; set; } = 0;
        public int ExamsTime { get; set; } = 0;
        public int CourseProjectsTime { get; set; } = 0;
        public int InterviewsTime { get; set; } = 0;
        public int TestsAndReferatsTime { get; set; } = 0;
        public int InternshipsTime { get; set; } = 0;
        public int DiplomasTime { get; set; } = 0;
        public int DiplomasReviewsTime { get; set; } = 0;
        public int SECTime { get; set; } = 0;
        public int GraduatesManagementTime { get; set; } = 0;
        public int GraduatesAcademicWorkTime { get; set; } = 0;
        public int PlasticPosesDemonstrationTime { get; set; } = 0;
        public int TestingEscortTime { get; set; } = 0;
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