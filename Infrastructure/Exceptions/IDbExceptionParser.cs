namespace Infrastructure.Exceptions;

public interface IDbExceptionParser
{
    /// <summary>
    /// Returns true when the exception represents a SQL unique/PK/duplicate-key violation.
    /// Optional index/constraint name can be provided for a fallback textual match.
    /// </summary>
    bool IsUniqueKeyViolation(Exception ex, string? indexOrConstraintName = null);
}
