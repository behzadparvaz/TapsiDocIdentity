using IdentityTapsiDoc.Identity.Core.Domain.Users.CommandSummery;
using MediatR;

namespace IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginWithTapsiSSO;

public class LoginWithTapsiSSOQuery : IRequest<RegisterSummery>
{
    public required string Code { get; set; }
}