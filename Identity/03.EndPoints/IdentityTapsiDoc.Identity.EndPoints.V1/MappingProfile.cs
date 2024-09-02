using AutoMapper;
using IdentityTapsiDoc.Identity.Core.Domain.DTOs.Services;
using IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService.Models;

namespace IdentityTapsiDoc.Identity.EndPoints.V1;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GetTokenOutput, LoginOutput>();
    }
}