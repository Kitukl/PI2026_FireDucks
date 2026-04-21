namespace Application.Helpers;

public static class UserProfileHelper
{
    public const string UserAvatarContainer = "user-avatars";
    public const int FeedbackSubjectMaxLength = 160;
    public const int FeedbackDescriptionMaxLength = 700;
    public const string FeedbackSubjectPrefix = "Subject: ";

    public static string ResolvePhotoUrl(string? photoUrl)
    {
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return "/images/no-photo.png";
        }

        var marker = UserAvatarContainer + "/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            var encoded = Uri.EscapeDataString(photoUrl);
            return $"/UserProfile/PhotoFile?path={encoded}";
        }

        return photoUrl;
    }

    public static bool TryExtractAvatarBlobName(string? photoUrl, out string blobName)
    {
        blobName = string.Empty;
        if (string.IsNullOrWhiteSpace(photoUrl))
        {
            return false;
        }

        var marker = UserAvatarContainer + "/";
        if (photoUrl.StartsWith(marker, StringComparison.OrdinalIgnoreCase))
        {
            blobName = photoUrl[marker.Length..];
            return !string.IsNullOrWhiteSpace(blobName);
        }

        if (!Uri.TryCreate(photoUrl, UriKind.Absolute, out var uri))
        {
            return false;
        }

        var absolutePath = uri.AbsolutePath.Replace('\\', '/');
        var markerIndex = absolutePath.IndexOf("/" + marker, StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            return false;
        }

        var start = markerIndex + marker.Length + 1;
        if (start >= absolutePath.Length)
        {
            return false;
        }

        blobName = absolutePath[start..];
        return !string.IsNullOrWhiteSpace(blobName);
    }

    public static string GetImageContentType(string blobName)
    {
        var extension = Path.GetExtension(blobName);
        return extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }

    public static string BuildFeedbackDescription(string? subject, string? description)
    {
        var normalizedSubject = (subject ?? string.Empty).Trim();
        var normalizedDescription = (description ?? string.Empty)
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Trim();

        if (normalizedSubject.Length > FeedbackSubjectMaxLength)
        {
            normalizedSubject = normalizedSubject[..FeedbackSubjectMaxLength];
        }

        if (string.IsNullOrWhiteSpace(normalizedDescription) && string.IsNullOrWhiteSpace(normalizedSubject))
        {
            return string.Empty;
        }

        string fullDescription;
        if (string.IsNullOrWhiteSpace(normalizedSubject))
        {
            fullDescription = normalizedDescription;
        }
        else
        {
            var maxBodyLength = FeedbackDescriptionMaxLength - FeedbackSubjectPrefix.Length - normalizedSubject.Length - 2;
            if (maxBodyLength < 0)
            {
                maxBodyLength = 0;
            }

            if (normalizedDescription.Length > maxBodyLength)
            {
                normalizedDescription = normalizedDescription[..maxBodyLength];
            }

            fullDescription = $"{FeedbackSubjectPrefix}{normalizedSubject}\n\n{normalizedDescription}";
        }

        if (fullDescription.Length > FeedbackDescriptionMaxLength)
        {
            fullDescription = fullDescription[..FeedbackDescriptionMaxLength];
        }

        return fullDescription;
    }
}
