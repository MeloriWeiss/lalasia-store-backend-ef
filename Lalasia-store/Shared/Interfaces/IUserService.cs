using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Controllers.Contracts.User;
using Lalasia_store.Models.Dto;

namespace Lalasia_store.Shared.Interfaces;

public interface IUserService
{
    public Task<UserDto> GetInfo(ClaimsPrincipal claimsPrincipal);
    public Task<DefaultResponse> ChangeInfo(ChangeUserInfoRequest request, ClaimsPrincipal claimsPrincipal);
    public Task<DefaultResponse> ChangePassword(ChangePasswordRequest request, ClaimsPrincipal claimsPrincipal);
}