﻿namespace Web.RequestModels.Contracts
{
    public class MonthReportViewModel
    {
        public int ContractID { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public bool IsBlocked { get; set; }
        public int LinkingPartID { get; set; }
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