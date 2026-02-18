namespace OnlineStore.Core.Models;

public enum SecurityEventType
{
    Login,
    Logout,
    Register,
    PasswordChange,
    RoleChange,
    TokenRefresh,
    FailedLogin,
    EmailConfirmation,
    PasswordResetRequest,
    PasswordReset,
    UserCreated,
    UserUpdated,
    UserDeleted,
    UserBlocked,
    RoleAssigned,
    RoleRevoked,
    SuspiciousActivity
}