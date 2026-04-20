using Application.Models;
using Moq;
using StudyHub.Core.Common;
using StudyHub.Core.DTOs;
using StudyHub.Core.Storage.DTOs;
using StudyHub.Core.Storage.Interfaces;

namespace StudyHub.UnitTests.Handlers.Common.Helpers;

public class StorageHelperTests
{
    private readonly Mock<IBlobService> _blobServiceMock;

    public StorageHelperTests()
    {
        _blobServiceMock = new Mock<IBlobService>();
    }

    [Fact]
    public async Task BuildStoragePageModelAsync_ShouldIncludePersonalAndGroupFiles_WhenUserHasGroup()
    {
        // Arrange
        _blobServiceMock.Reset();

        var user = new UserDto
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "User",
            Surname = "Test",
            Roles = new List<string> { "Student" },
            GroupName = "PI-24"
        };

        _blobServiceMock
            .Setup(service => service.ListFilesAsync("user-storage-11111111-1111-1111-1111-111111111111", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BlobFileInfoDto>
            {
                new() { Name = "a1b2c3d4__personal.png", Size = 1024, LastModified = DateTimeOffset.UtcNow }
            });

        _blobServiceMock
            .Setup(service => service.ListFilesAsync("group-storage-PI-24", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BlobFileInfoDto>
            {
                new() { Name = "d4c3b2a1__group.pdf", Size = 2048, LastModified = DateTimeOffset.UtcNow.AddMinutes(-10) }
            });

        // Act
        var result = await StorageHelper.BuildStoragePageModelAsync(user, _blobServiceMock.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PI-24", result.GroupName);
        Assert.True(result.CanShareWithGroup);
        Assert.Equal(2, result.Files.Count);
        Assert.Contains(result.Files, file => file.Scope == "personal" && file.Name == "personal.png");
        Assert.Contains(result.Files, file => file.Scope == "group" && file.Name == "group.pdf");
    }

    [Fact]
    public async Task BuildStoragePageModelAsync_ShouldIncludeOnlyPersonalFiles_WhenUserHasNoGroup()
    {
        // Arrange
        _blobServiceMock.Reset();

        var user = new UserDto
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "User",
            Surname = "Test",
            Roles = new List<string> { "Student" },
            GroupName = string.Empty
        };

        _blobServiceMock
            .Setup(service => service.ListFilesAsync("user-storage-22222222-2222-2222-2222-222222222222", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BlobFileInfoDto>
            {
                new() { Name = "abc123__note.txt", Size = 99, LastModified = DateTimeOffset.UtcNow }
            });

        // Act
        var result = await StorageHelper.BuildStoragePageModelAsync(user, _blobServiceMock.Object, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.CanShareWithGroup);
        Assert.Single(result.Files);
        Assert.Equal("note.txt", result.Files[0].Name);
        Assert.Equal("TXT", result.Files[0].Extension);
    }
}
