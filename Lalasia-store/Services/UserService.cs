using System.Security.Claims;
using Lalasia_store.Controllers.Contracts.Common;
using Lalasia_store.Controllers.Contracts.User;
using Lalasia_store.Models.Data;
using Lalasia_store.Models.Dto;
using Lalasia_store.Shared.Exceptions;
using Lalasia_store.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lalasia_store.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(
        UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDto> GetInfo(ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        return new UserDto() { Name = user.UserName, Email = user.Email, Phone = user.PhoneNumber ?? "" };
    }

    public async Task<DefaultResponse> ChangeInfo(ChangeUserInfoRequest request, ClaimsPrincipal claimsPrincipal)
    {
        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        if (request.Name is not null)
        {
            user.UserName = request.Name;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new BadRequestException("Couldn't change user name");
        }

        if (request.Phone is not null)
        {
            var existingPhone =
                await _userManager.Users.FirstOrDefaultAsync(usr => usr.PhoneNumber == request.Phone);

            if (existingPhone is not null)
                throw new BadRequestException("The phone number is already in use");

            var result = await _userManager.ChangePhoneNumberAsync(user, request.Phone,
                await _userManager.GenerateChangePhoneNumberTokenAsync(user, request.Phone));

            if (!result.Succeeded)
                throw new BadRequestException("Couldn't change phone number");
        }

        if (request.Email is not null)
        {
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingEmail is not null)
                throw new BadRequestException("The email is already in use");

            var result = await _userManager.ChangeEmailAsync(user, request.Email,
                await _userManager.GenerateChangeEmailTokenAsync(user, request.Email));

            if (!result.Succeeded)
                throw new BadRequestException("Couldn't change email");
        }

        return new DefaultResponse() { Error = false, Message = "Data changed successfully" };
    }

    public async Task<DefaultResponse> ChangePassword(ChangePasswordRequest request, ClaimsPrincipal claimsPrincipal)
    {
        if (request.OldPassword == request.Password)
            throw new BadRequestException("The new and old passwords must not match");

        var user = await _userManager.GetUserAsync(claimsPrincipal);

        if (user is null)
            throw new NotFoundException("The user was not found");

        var isCorrectOldPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword);

        if (!isCorrectOldPassword)
            throw new BadRequestException("The password is incorrect");

        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.Password);

        if (!result.Succeeded)
            throw new BadRequestException("Couldn't change password");

        return new DefaultResponse() { Error = false, Message = "Password changed successfully" };
    }
}