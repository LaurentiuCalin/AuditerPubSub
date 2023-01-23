using AuditingPubApi.Domain.Exceptions;
using AuditingPubApi.Domain.Models;
using AuditingPubApi.Requests;
using Contracts;
using MessageBus.Interfaces;

namespace AuditingPubApi.Domain.Services;

public class UserService : IUserService
{
    public readonly IDictionary<string, User> Users;
    private readonly IEventPublisher _eventPublisher;

    public UserService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        Users = new Dictionary<string, User>();
    }

    public async Task<User> Create(CreateUserRequest createUserRequest)
    {
        if (string.IsNullOrWhiteSpace(createUserRequest.Name) || string.IsNullOrWhiteSpace(createUserRequest.Email))
        {
            throw new ValidationException("Both the name and the email are required");
        }

        if (Users.ContainsKey(createUserRequest.Email))
        {
            throw new ValidationException("The email is already in use");
        }

        var user = new User(createUserRequest.Email, createUserRequest.Name);

        Users.Add(createUserRequest.Email, user);

        _eventPublisher.PublishEvent(new UserCreatedEvent(user.Name, user.Email));

        return await Task.FromResult(user);
    }
}