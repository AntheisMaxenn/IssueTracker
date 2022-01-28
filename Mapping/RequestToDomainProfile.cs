using AutoMapper;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.DTO;

namespace IssueTracker.Mapping
{
    public class RequestToDomainProfile : Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<PaginationQuery, PaginationFilter>();
            CreateMap<NameDescriptionRequest, Machine>();
            CreateMap<NameDescriptionRequest, Location>();
            CreateMap<NameDescriptionRequest, Component>();
            
            CreateMap<MachineDTO, Machine>();
            CreateMap<LocationDTO, Location>();
            CreateMap<ComponentDTO, Component>();
            CreateMap<ActionRequest, Data.Action>();
            
            
            CreateMap<EmployeeRegistrationRequest, Employee>();
            CreateMap<IssueRequest, Issue>();



        }

    }
}
