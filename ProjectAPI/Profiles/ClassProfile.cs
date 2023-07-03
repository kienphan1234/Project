using AutoMapper;
using ProjectAPI.Models;
using ProjectAPI.ModelsDTO;

namespace ProjectAPI.Profiles
{
    public class ClassProfile : Profile
    {
        public ClassProfile()
        {
            CreateMap<Class, ClassDTO>()
                .ForMember(dest =>
                dest.Teacher,
                opt => opt.MapFrom(src => src.Teacher.Name));
        }
    }
}
