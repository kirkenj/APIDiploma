using AutoMapper;
using Database.Entities;
using Logic.Models.MonthReports;
using Web.Models.AcademicDegrees;
using Web.Models.Contracts;
using Web.Models.ContractType;
using Web.Models.Departments;
using Web.RequestModels.Account;
using Web.RequestModels.Authorize;
using Web.RequestModels.Contracts;

namespace API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Contract
            CreateMap<ContractCreateModel, Contract>()
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ContractIdentifier, opt => opt.MapFrom(src => src.ContractIdentifier))
            .ForMember(dest => dest.PeriodStart, opt => opt.MapFrom(src => src.PeriodStart))
            .ForMember(dest => dest.PeriodEnd, opt => opt.MapFrom(src => src.PeriodEnd))
            .ForMember(dest => dest.DepartmentID, opt => opt.MapFrom(src => src.DepartmentID))
            .ForMember(dest => dest.ContractTypeID, opt => opt.MapFrom(src => src.ContractTypeID))
            .ForMember(dest => dest.ParentContractID, opt => opt.MapFrom(src => src.ParentContractID))
            .ForMember(dest => dest.TestingEscortMaxTime, opt => opt.MapFrom(src => src.TestingEscortMaxTime))
            .ForMember(dest => dest.PlasticPosesDemonstrationMaxTime, opt => opt.MapFrom(src => src.PlasticPosesDemonstrationMaxTime))
            .ForMember(dest => dest.GraduatesAcademicWorkMaxTime, opt => opt.MapFrom(src => src.GraduatesAcademicWorkMaxTime))
            .ForMember(dest => dest.GraduatesManagementMaxTime, opt => opt.MapFrom(src => src.GraduatesManagementMaxTime))
            .ForMember(dest => dest.SECMaxTime, opt => opt.MapFrom(src => src.SECMaxTime))
            .ForMember(dest => dest.DiplomasReviewsMaxTime, opt => opt.MapFrom(src => src.DiplomasReviewsMaxTime))
            .ForMember(dest => dest.DiplomasMaxTime, opt => opt.MapFrom(src => src.DiplomasMaxTime))
            .ForMember(dest => dest.InternshipsMaxTime, opt => opt.MapFrom(src => src.InternshipsMaxTime))
            .ForMember(dest => dest.TestsAndReferatsMaxTime, opt => opt.MapFrom(src => src.TestsAndReferatsMaxTime))
            .ForMember(dest => dest.InterviewsMaxTime, opt => opt.MapFrom(src => src.InterviewsMaxTime))
            .ForMember(dest => dest.LectionsMaxTime, opt => opt.MapFrom(src => src.LectionsMaxTime))
            .ForMember(dest => dest.PracticalClassesMaxTime, opt => opt.MapFrom(src => src.PracticalClassesMaxTime))
            .ForMember(dest => dest.LaboratoryClassesMaxTime, opt => opt.MapFrom(src => src.LaboratoryClassesMaxTime))
            .ForMember(dest => dest.ConsultationsMaxTime, opt => opt.MapFrom(src => src.ConsultationsMaxTime))
            .ForMember(dest => dest.OtherTeachingClassesMaxTime, opt => opt.MapFrom(src => src.OtherTeachingClassesMaxTime))
            .ForMember(dest => dest.CreditsMaxTime, opt => opt.MapFrom(src => src.CreditsMaxTime))
            .ForMember(dest => dest.ExamsMaxTime, opt => opt.MapFrom(src => src.ExamsMaxTime))
            .ForMember(dest => dest.CourseProjectsMaxTime, opt => opt.MapFrom(src => src.CourseProjectsMaxTime));
            
            CreateMap<ContractEditModel, Contract>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ContractID))
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ContractIdentifier, opt => opt.MapFrom(src => src.ContractIdentifier))
            .ForMember(dest => dest.PeriodStart, opt => opt.MapFrom(src => src.PeriodStart))
            .ForMember(dest => dest.PeriodEnd, opt => opt.MapFrom(src => src.PeriodEnd))
            .ForMember(dest => dest.DepartmentID, opt => opt.MapFrom(src => src.DepartmentID))
            .ForMember(dest => dest.ContractTypeID, opt => opt.MapFrom(src => src.ContractTypeID))
            .ForMember(dest => dest.ParentContractID, opt => opt.MapFrom(src => src.ParentContractID))
            .ForMember(dest => dest.TestingEscortMaxTime, opt => opt.MapFrom(src => src.TestingEscortMaxTime))
            .ForMember(dest => dest.PlasticPosesDemonstrationMaxTime, opt => opt.MapFrom(src => src.PlasticPosesDemonstrationMaxTime))
            .ForMember(dest => dest.GraduatesAcademicWorkMaxTime, opt => opt.MapFrom(src => src.GraduatesAcademicWorkMaxTime))
            .ForMember(dest => dest.GraduatesManagementMaxTime, opt => opt.MapFrom(src => src.GraduatesManagementMaxTime))
            .ForMember(dest => dest.SECMaxTime, opt => opt.MapFrom(src => src.SECMaxTime))
            .ForMember(dest => dest.DiplomasReviewsMaxTime, opt => opt.MapFrom(src => src.DiplomasReviewsMaxTime))
            .ForMember(dest => dest.DiplomasMaxTime, opt => opt.MapFrom(src => src.DiplomasMaxTime))
            .ForMember(dest => dest.InternshipsMaxTime, opt => opt.MapFrom(src => src.InternshipsMaxTime))
            .ForMember(dest => dest.TestsAndReferatsMaxTime, opt => opt.MapFrom(src => src.TestsAndReferatsMaxTime))
            .ForMember(dest => dest.InterviewsMaxTime, opt => opt.MapFrom(src => src.InterviewsMaxTime))
            .ForMember(dest => dest.LectionsMaxTime, opt => opt.MapFrom(src => src.LectionsMaxTime))
            .ForMember(dest => dest.PracticalClassesMaxTime, opt => opt.MapFrom(src => src.PracticalClassesMaxTime))
            .ForMember(dest => dest.LaboratoryClassesMaxTime, opt => opt.MapFrom(src => src.LaboratoryClassesMaxTime))
            .ForMember(dest => dest.ConsultationsMaxTime, opt => opt.MapFrom(src => src.ConsultationsMaxTime))
            .ForMember(dest => dest.OtherTeachingClassesMaxTime, opt => opt.MapFrom(src => src.OtherTeachingClassesMaxTime))
            .ForMember(dest => dest.CreditsMaxTime, opt => opt.MapFrom(src => src.CreditsMaxTime))
            .ForMember(dest => dest.ExamsMaxTime, opt => opt.MapFrom(src => src.ExamsMaxTime))
            .ForMember(dest => dest.CourseProjectsMaxTime, opt => opt.MapFrom(src => src.CourseProjectsMaxTime));


            CreateMap<Contract, ContractViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
            .ForMember(dest => dest.ContractIdentifier, opt => opt.MapFrom(src => src.ContractIdentifier))
            .ForMember(dest => dest.DepartmentID, opt => opt.MapFrom(src => src.DepartmentID))
            .ForMember(dest => dest.LinkingPartID, opt => opt.MapFrom(src => src.LinkingPartID))
            .ForMember(dest => dest.ParentContractID, opt => opt.MapFrom(src => src.ParentContractID))
            .ForMember(dest => dest.ContractTypeID, opt => opt.MapFrom(src => src.ContractTypeID))
            .ForMember(dest => dest.PeriodStart, opt => opt.MapFrom(src => src.PeriodStart))
            .ForMember(dest => dest.PeriodEnd, opt => opt.MapFrom(src => src.PeriodEnd))
            .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => src.IsConfirmed))
            .ForMember(dest => dest.TestingEscortMaxTime, opt => opt.MapFrom(src => src.TestingEscortMaxTime))
            .ForMember(dest => dest.PlasticPosesDemonstrationMaxTime, opt => opt.MapFrom(src => src.PlasticPosesDemonstrationMaxTime))
            .ForMember(dest => dest.GraduatesAcademicWorkMaxTime, opt => opt.MapFrom(src => src.GraduatesAcademicWorkMaxTime))
            .ForMember(dest => dest.GraduatesManagementMaxTime, opt => opt.MapFrom(src => src.GraduatesManagementMaxTime))
            .ForMember(dest => dest.SECMaxTime, opt => opt.MapFrom(src => src.SECMaxTime))
            .ForMember(dest => dest.DiplomasReviewsMaxTime, opt => opt.MapFrom(src => src.DiplomasReviewsMaxTime))
            .ForMember(dest => dest.DiplomasMaxTime, opt => opt.MapFrom(src => src.DiplomasMaxTime))
            .ForMember(dest => dest.InternshipsMaxTime, opt => opt.MapFrom(src => src.InternshipsMaxTime))
            .ForMember(dest => dest.TestsAndReferatsMaxTime, opt => opt.MapFrom(src => src.TestsAndReferatsMaxTime))
            .ForMember(dest => dest.InterviewsMaxTime, opt => opt.MapFrom(src => src.InterviewsMaxTime))
            .ForMember(dest => dest.LectionsMaxTime, opt => opt.MapFrom(src => src.LectionsMaxTime))
            .ForMember(dest => dest.PracticalClassesMaxTime, opt => opt.MapFrom(src => src.PracticalClassesMaxTime))
            .ForMember(dest => dest.LaboratoryClassesMaxTime, opt => opt.MapFrom(src => src.LaboratoryClassesMaxTime))
            .ForMember(dest => dest.ConsultationsMaxTime, opt => opt.MapFrom(src => src.ConsultationsMaxTime))
            .ForMember(dest => dest.OtherTeachingClassesMaxTime, opt => opt.MapFrom(src => src.OtherTeachingClassesMaxTime))
            .ForMember(dest => dest.CreditsMaxTime, opt => opt.MapFrom(src => src.CreditsMaxTime))
            .ForMember(dest => dest.ExamsMaxTime, opt => opt.MapFrom(src => src.ExamsMaxTime))
            .ForMember(dest => dest.CourseProjectsMaxTime, opt => opt.MapFrom(src => src.CourseProjectsMaxTime));
            
            CreateMap<KeyValuePair<Contract, bool>, ContractViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Key.ID))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Key.UserID))
            .ForMember(dest => dest.ContractIdentifier, opt => opt.MapFrom(src => src.Key.ContractIdentifier))
            .ForMember(dest => dest.DepartmentID, opt => opt.MapFrom(src => src.Key.DepartmentID))
            .ForMember(dest => dest.LinkingPartID, opt => opt.MapFrom(src => src.Key.LinkingPartID))
            .ForMember(dest => dest.ParentContractID, opt => opt.MapFrom(src => src.Key.ParentContractID))
            .ForMember(dest => dest.ContractTypeID, opt => opt.MapFrom(src => src.Key.ContractTypeID))
            .ForMember(dest => dest.PeriodStart, opt => opt.MapFrom(src => src.Key.PeriodStart))
            .ForMember(dest => dest.PeriodEnd, opt => opt.MapFrom(src => src.Key.PeriodEnd))
            .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => src.Key.IsConfirmed))
            .ForMember(dest => dest.TestingEscortMaxTime, opt => opt.MapFrom(src => src.Key.TestingEscortMaxTime))
            .ForMember(dest => dest.PlasticPosesDemonstrationMaxTime, opt => opt.MapFrom(src => src.Key.PlasticPosesDemonstrationMaxTime))
            .ForMember(dest => dest.GraduatesAcademicWorkMaxTime, opt => opt.MapFrom(src => src.Key.GraduatesAcademicWorkMaxTime))
            .ForMember(dest => dest.GraduatesManagementMaxTime, opt => opt.MapFrom(src => src.Key.GraduatesManagementMaxTime))
            .ForMember(dest => dest.SECMaxTime, opt => opt.MapFrom(src => src.Key.SECMaxTime))
            .ForMember(dest => dest.DiplomasReviewsMaxTime, opt => opt.MapFrom(src => src.Key.DiplomasReviewsMaxTime))
            .ForMember(dest => dest.DiplomasMaxTime, opt => opt.MapFrom(src => src.Key.DiplomasMaxTime))
            .ForMember(dest => dest.InternshipsMaxTime, opt => opt.MapFrom(src => src.Key.InternshipsMaxTime))
            .ForMember(dest => dest.TestsAndReferatsMaxTime, opt => opt.MapFrom(src => src.Key.TestsAndReferatsMaxTime))
            .ForMember(dest => dest.InterviewsMaxTime, opt => opt.MapFrom(src => src.Key.InterviewsMaxTime))
            .ForMember(dest => dest.LectionsMaxTime, opt => opt.MapFrom(src => src.Key.LectionsMaxTime))
            .ForMember(dest => dest.PracticalClassesMaxTime, opt => opt.MapFrom(src => src.Key.PracticalClassesMaxTime))
            .ForMember(dest => dest.LaboratoryClassesMaxTime, opt => opt.MapFrom(src => src.Key.LaboratoryClassesMaxTime))
            .ForMember(dest => dest.ConsultationsMaxTime, opt => opt.MapFrom(src => src.Key.ConsultationsMaxTime))
            .ForMember(dest => dest.OtherTeachingClassesMaxTime, opt => opt.MapFrom(src => src.Key.OtherTeachingClassesMaxTime))
            .ForMember(dest => dest.CreditsMaxTime, opt => opt.MapFrom(src => src.Key.CreditsMaxTime))
            .ForMember(dest => dest.ExamsMaxTime, opt => opt.MapFrom(src => src.Key.ExamsMaxTime))
            .ForMember(dest => dest.CourseProjectsMaxTime, opt => opt.MapFrom(src => src.Key.CourseProjectsMaxTime))
            .ForMember(dest => dest.HasChild, opt => opt.MapFrom(src => src.Value));
            #endregion

            #region MonthReport
            CreateMap<MonthReport, MonthReportViewModel>()
            .ForMember(dest => dest.LinkingPartID, opt => opt.MapFrom(src => src.LinkingPartID))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.Month))
            .ForMember(dest => dest.TestingEscortTime, opt => opt.MapFrom(src => src.TestingEscortTime))
            .ForMember(dest => dest.IsBlocked, opt => opt.MapFrom(src => src.IsBlocked))
            .ForMember(dest => dest.PlasticPosesDemonstrationTime, opt => opt.MapFrom(src => src.PlasticPosesDemonstrationTime))
            .ForMember(dest => dest.GraduatesAcademicWorkTime, opt => opt.MapFrom(src => src.GraduatesAcademicWorkTime))
            .ForMember(dest => dest.GraduatesManagementTime, opt => opt.MapFrom(src => src.GraduatesManagementTime))
            .ForMember(dest => dest.DiplomasReviewsTime, opt => opt.MapFrom(src => src.DiplomasReviewsTime))
            .ForMember(dest => dest.DiplomasTime, opt => opt.MapFrom(src => src.DiplomasTime))
            .ForMember(dest => dest.InternshipsTime, opt => opt.MapFrom(src => src.InternshipsTime))
            .ForMember(dest => dest.TestsAndReferatsTime, opt => opt.MapFrom(src => src.TestsAndReferatsTime))
            .ForMember(dest => dest.InterviewsTime, opt => opt.MapFrom(src => src.InterviewsTime))
            .ForMember(dest => dest.LectionsTime, opt => opt.MapFrom(src => src.LectionsTime))
            .ForMember(dest => dest.PracticalClassesTime, opt => opt.MapFrom(src => src.PracticalClassesTime))
            .ForMember(dest => dest.LaboratoryClassesTime, opt => opt.MapFrom(src => src.LaboratoryClassesTime))
            .ForMember(dest => dest.ConsultationsTime, opt => opt.MapFrom(src => src.ConsultationsTime))
            .ForMember(dest => dest.OtherTeachingClassesTime, opt => opt.MapFrom(src => src.OtherTeachingClassesTime))
            .ForMember(dest => dest.CreditsTime, opt => opt.MapFrom(src => src.CreditsTime))
            .ForMember(dest => dest.ExamsTime, opt => opt.MapFrom(src => src.ExamsTime))
            .ForMember(dest => dest.CourseProjectsTime, opt => opt.MapFrom(src => src.CourseProjectsTime))
            .ForMember(dest => dest.SECTime, opt => opt.MapFrom(src => src.SECTime));
            
            CreateMap<EditMonthReportModel, MonthReport> ()
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.Month))
            .ForMember(dest => dest.TestingEscortTime, opt => opt.MapFrom(src => src.TestingEscortTime))
            .ForMember(dest => dest.PlasticPosesDemonstrationTime, opt => opt.MapFrom(src => src.PlasticPosesDemonstrationTime))
            .ForMember(dest => dest.GraduatesAcademicWorkTime, opt => opt.MapFrom(src => src.GraduatesAcademicWorkTime))
            .ForMember(dest => dest.GraduatesManagementTime, opt => opt.MapFrom(src => src.GraduatesManagementTime))
            .ForMember(dest => dest.DiplomasReviewsTime, opt => opt.MapFrom(src => src.DiplomasReviewsTime))
            .ForMember(dest => dest.DiplomasTime, opt => opt.MapFrom(src => src.DiplomasTime))
            .ForMember(dest => dest.InternshipsTime, opt => opt.MapFrom(src => src.InternshipsTime))
            .ForMember(dest => dest.TestsAndReferatsTime, opt => opt.MapFrom(src => src.TestsAndReferatsTime))
            .ForMember(dest => dest.InterviewsTime, opt => opt.MapFrom(src => src.InterviewsTime))
            .ForMember(dest => dest.LectionsTime, opt => opt.MapFrom(src => src.LectionsTime))
            .ForMember(dest => dest.PracticalClassesTime, opt => opt.MapFrom(src => src.PracticalClassesTime))
            .ForMember(dest => dest.LaboratoryClassesTime, opt => opt.MapFrom(src => src.LaboratoryClassesTime))
            .ForMember(dest => dest.ConsultationsTime, opt => opt.MapFrom(src => src.ConsultationsTime))
            .ForMember(dest => dest.OtherTeachingClassesTime, opt => opt.MapFrom(src => src.OtherTeachingClassesTime))
            .ForMember(dest => dest.CreditsTime, opt => opt.MapFrom(src => src.CreditsTime))
            .ForMember(dest => dest.ExamsTime, opt => opt.MapFrom(src => src.ExamsTime))
            .ForMember(dest => dest.CourseProjectsTime, opt => opt.MapFrom(src => src.CourseProjectsTime))
            .ForMember(dest => dest.SECTime, opt => opt.MapFrom(src => src.SECTime));
            #endregion

            #region User
            CreateMap<RegisterUserModel, User>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
            .ForMember(dest => dest.Patronymic, opt => opt.MapFrom(src => src.Patronymic))
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordStr))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login));
            
            CreateMap<User, UserViewModel>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
            .ForMember(dest => dest.Patronymic, opt => opt.MapFrom(src => src.Patronymic))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login));
            
            CreateMap<UserEditModel, User>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
            .ForMember(dest => dest.Patronymic, opt => opt.MapFrom(src => src.Patronymic))
            .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleID))
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login));
            #endregion

            #region Department
            CreateMap<DepartmentViewModel, Department>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

            CreateMap<Department, DepartmentViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));
            #endregion

            #region AcademicDegree
            CreateMap<AcademicDegree, AcademicDegreeViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

            CreateMap<AcademicDegreeViewModel, AcademicDegree>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));
            #endregion

            #region ContractType
            CreateMap<ContractType, ContractTypeViewModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));

            CreateMap<ContractTypeViewModel, ContractType>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID));
            #endregion
        }
    }
}
