using AutoMapper;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.DTO;

namespace IssueTracker.Mapping
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Machine, MachineDTO>();
            CreateMap<Component, ComponentDTO>();
            CreateMap<Location, LocationDTO>();
        }
    }
}
