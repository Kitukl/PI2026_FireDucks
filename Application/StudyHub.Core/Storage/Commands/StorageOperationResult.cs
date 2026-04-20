namespace StudyHub.Core.Storage.Commands;

public class StorageOperationResult
{
    public bool IsSuccess { get; set; }
    public bool IsForbidden { get; set; }
    public string? Message { get; set; }

    public static StorageOperationResult Success(string message) => new() { IsSuccess = true, Message = message };
    public static StorageOperationResult Fail(string message) => new() { IsSuccess = false, Message = message };
    public static StorageOperationResult Forbidden() => new() { IsForbidden = true, IsSuccess = false };
}