namespace ProjectManagementSystem.Application.Common
{
    public sealed record ApiResponse<T>(bool Succeeded, T? Data, string? Error)
    {
        public static ApiResponse<T> Success(T data) => new(true, data, null);

        public static ApiResponse<T> Failure(string error) => new(false, default, error);
    }
}
