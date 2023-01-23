using AuditingPubApi.Domain.Models;
using AuditingPubApi.Requests;

namespace AuditingPubApi.Domain.Services;

public interface IUserService
{
    Task<User> Create(CreateUserRequest createUserRequest);
}