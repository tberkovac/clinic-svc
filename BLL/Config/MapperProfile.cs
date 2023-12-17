using AutoMapper;

namespace BLL.Config;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<DAL.Entities.User, Dto.UserDto>().ReverseMap();
        CreateMap<DAL.Entities.Patient, Dto.PatientDto>().ReverseMap();
        CreateMap<DAL.Entities.Doctor, Dto.DoctorDto>().ReverseMap();
        CreateMap<DAL.Entities.Title, Dto.TitleDto>().ReverseMap();
        CreateMap<DAL.Entities.Role, Dto.RoleDto>().ReverseMap();
        CreateMap<DAL.Entities.Admission, Dto.AdmissionDto>().ReverseMap();
        CreateMap<DAL.Entities.Record, Dto.RecordDto>().ReverseMap();
        CreateMap<DAL.Entities.LeaveRequest, Dto.LeaveRequestDto>().ReverseMap();
        CreateMap<DAL.Entities.SearchParams, Dto.SearchParamsDto>().ReverseMap();
        CreateMap<DAL.Entities.SearchAdmissionParams, Dto.SearchAdmissionsParamsDto>();
        CreateMap(typeof(DAL.Entities.ResponsePage<>), typeof(Dto.ResponsePageDto<>)).ReverseMap();
    }
}