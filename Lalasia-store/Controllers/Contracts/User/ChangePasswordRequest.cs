namespace Lalasia_store.Controllers.Contracts.User;

public record ChangePasswordRequest(string OldPassword, string Password);