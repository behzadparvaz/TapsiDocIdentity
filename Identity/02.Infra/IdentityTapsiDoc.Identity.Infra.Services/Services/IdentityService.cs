using AutoMapper;
using IdentityTapsiDoc.Identity.Core.Domain.Contracts.Services;
using IdentityTapsiDoc.Identity.Core.Domain.DTOs.Services;
using IdentityTapsiDoc.Identity.Core.Domain.Enums;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Infra.Services.Services.ServiceImpls.IdentityService;
using Microsoft.AspNetCore.Identity;

namespace IdentityTapsiDoc.Identity.Infra.Services.Services;

public class IdentityService : IIdentityService
{
    private UserManager<User> _userManager;
    private IIdentityServiceFactory _identityServiceFactory;
    IMapper _mapper;

    public IdentityService(UserManager<User> userManager, IIdentityServiceFactory identityServiceFactory, IMapper mapper)
    {
        _userManager = userManager;
        _identityServiceFactory = identityServiceFactory;
        _mapper = mapper;
    }

    public async Task<string> GetUserInfo(string accessToken)
    {
        var strategy = _identityServiceFactory.GetIdentityServiceStrategy(AuthenticationType.TapsiSso);
        var res = await strategy.GetUserInfo(accessToken);
        return res.PhoneNumber;
    }

    public async Task<LoginOutput> Login(string userName, AuthenticationType authType)
    {
        var strategy = _identityServiceFactory.GetIdentityServiceStrategy(authType);
        var tokenOutput = await strategy.GetToken(userName);
        var result = _mapper.Map<LoginOutput>(tokenOutput);
        return result;

    }
}