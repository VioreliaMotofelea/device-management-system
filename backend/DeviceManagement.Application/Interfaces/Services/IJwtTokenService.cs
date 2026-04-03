using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Application.Interfaces.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(User user);
}
