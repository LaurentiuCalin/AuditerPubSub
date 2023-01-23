using AuditingPubApi.Domain.Exceptions;
using AuditingPubApi.Domain.Models;
using AuditingPubApi.Domain.Services;
using AuditingPubApi.Requests;
using MessageBus.Interfaces;
using Moq;
using Xunit;

namespace AuditingPubApi.Tests.Domain.Services;

public class UserServiceTests
{
    [Fact]
    public async Task Create_ThrowsValidationException_WhenRequestContainsEmptyParameter()
    {
        // Arrange
        var sut = new UserService(Mock.Of<IEventPublisher>());

        // Act && Assert
        await Assert.ThrowsAsync<ValidationException>(async () => await sut.Create(new CreateUserRequest("", "email")));
    }
    
    [Fact]
    public async Task Create_CreatesCorrectEntry_WhenRequestIsValid()
    {
        // Arrange
        var sut = new UserService(Mock.Of<IEventPublisher>());
        var expectedUser = new User("name", "email");
        
        // Act
        await sut.Create(new CreateUserRequest(expectedUser.Name, expectedUser.Email));
        
        // Assert
        Assert.True(sut.Users.ContainsKey(expectedUser.Email));
        Assert.Equal(1, sut.Users.Count);
    }
}