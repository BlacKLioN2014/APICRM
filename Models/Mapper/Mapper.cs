using AutoMapper;

namespace APICRM.Models.Mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Models.returnRequest, APICRM.Models.Mapper.returnRequest>().ReverseMap();
            CreateMap<Models.DocumentReferences, APICRM.Models.Mapper.DocumentReferences>().ReverseMap();
            CreateMap<Models.DocumentLines, APICRM.Models.Mapper.DocumentLines>().ReverseMap();
        }
    }
}
