using StudyHub.Core.Common;

namespace StudyHub.UnitTests.Handlers.Common.Helpers;

public class StorageNamingHelperTests
{
    [Fact]
    public void BuildUserStorageContainerName_ShouldIncludePrefixAndGuid_WhenUserIdIsValid()
    {
        // Arrange
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var result = StorageNamingHelper.BuildUserStorageContainerName(userId);

        // Assert
        Assert.Equal("user-storage-11111111-1111-1111-1111-111111111111", result);
    }

    [Fact]
    public void StripStoredFilePrefix_ShouldReturnOriginalFileName_WhenStoredPrefixExists()
    {
        // Arrange
        const string blobName = "4ac4c5fcb5d8438aab66d79f91ddcf10__report.pdf";

        // Act
        var result = StorageNamingHelper.StripStoredFilePrefix(blobName);

        // Assert
        Assert.Equal("report.pdf", result);
    }

    [Fact]
    public void GetContentTypeByFileName_ShouldReturnKnownMimeType_WhenExtensionIsPng()
    {
        // Arrange
        const string fileName = "avatar.png";

        // Act
        var result = StorageNamingHelper.GetContentTypeByFileName(fileName);

        // Assert
        Assert.Equal("image/png", result);
    }
}
