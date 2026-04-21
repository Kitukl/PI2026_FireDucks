using Application.Helpers;

namespace StudyHub.UnitTests.Handlers.Common.Helpers;

public class UserProfileHelperTests
{
    [Fact]
    public void ResolvePhotoUrl_ShouldReturnDefaultImage_WhenPhotoUrlIsEmpty()
    {
        // Arrange
        const string? photoUrl = null;

        // Act
        var result = UserProfileHelper.ResolvePhotoUrl(photoUrl);

        // Assert
        Assert.Equal("/images/no-photo.png", result);
    }

    [Fact]
    public void ResolvePhotoUrl_ShouldReturnPhotoFileEndpoint_WhenPhotoUrlIsAvatarBlobPath()
    {
        // Arrange
        const string photoUrl = "user-avatars/my-image.png";

        // Act
        var result = UserProfileHelper.ResolvePhotoUrl(photoUrl);

        // Assert
        Assert.StartsWith("/UserProfile/PhotoFile?path=", result);
        Assert.Contains("user-avatars%2Fmy-image.png", result);
    }

    [Fact]
    public void TryExtractAvatarBlobName_ShouldReturnTrue_WhenPhotoUrlContainsBlobName()
    {
        // Arrange
        const string photoUrl = "user-avatars/avatar.jpeg";

        // Act
        var isSuccess = UserProfileHelper.TryExtractAvatarBlobName(photoUrl, out var blobName);

        // Assert
        Assert.True(isSuccess);
        Assert.Equal("avatar.jpeg", blobName);
    }

    [Fact]
    public void BuildFeedbackDescription_ShouldIncludeSubjectPrefix_WhenSubjectAndDescriptionProvided()
    {
        // Arrange
        const string subject = "Login issue";
        const string description = "Cannot sign in from mobile.";

        // Act
        var result = UserProfileHelper.BuildFeedbackDescription(subject, description);

        // Assert
        Assert.StartsWith("Subject: Login issue", result);
        Assert.Contains("Cannot sign in from mobile.", result);
    }
}
