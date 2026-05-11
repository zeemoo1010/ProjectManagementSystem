namespace ProjectManagementSystem.Application.Common
{
    public sealed record Result<T>(bool Succeeded, T? Value, string? Error)
    {
        public static Result<T> Success(T value) => new(true, value, null);

        public static Result<T> Failure(string error) => new(false, default, error);

        public ApiResponse<T> ToApiResponse() =>
            Succeeded && Value is not null
                ? ApiResponse<T>.Success(Value)
                : ApiResponse<T>.Failure(Error ?? "The operation failed.");
    }
}
