using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.RegisterUser;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.SetPassword;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Commands.Verification;
using IdentityTapsiDoc.Identity.Core.ApplicationService.Users.Queries.LoginUser;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace IdentityTapsiDoc.Identity.EndPoints.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator mediator;

        public AccountsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("/[controller]/Register")]
        public async Task<IActionResult> Post([FromBody] RegisterUserCommand command)
        {
            try
            {
                var result = await this.mediator.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new {StatusCode = 500 , Message = ex.Message });
            }

        }

        [HttpPost("/[controller]/VerifyCode")]
        public async Task<IActionResult> Post([FromBody] VerificationCommand command)
        {
            var result = await this.mediator.Send(command);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("/[controller]/SetPassword")]
        public async Task<IActionResult> Post([FromBody] SetPasswordCommand command)
        {
            var result = await this.mediator.Send(command);
            return Ok(result);
        }


        [HttpPost("/[controller]/Login")]
        public async Task<IActionResult> Post([FromBody] LoginUserQuery query)
        {
            try
            {
                var result = await this.mediator.Send(query);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(new { StatusCode = 500, Message = ex.Message });
            }

        }


    }
}
