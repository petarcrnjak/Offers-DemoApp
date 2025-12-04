using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Exceptions;

public class DbExceptionParser : IDbExceptionParser
{
    public bool IsUniqueKeyViolation(Exception ex, string? indexOrConstraintName = null)
    {
        if (ex == null) return false;

        if (ex is DbUpdateException dbUpEx && dbUpEx.InnerException is SqlException sqlEx)
            return sqlEx.Number is 2627 or 2601;

        if (ex.InnerException is SqlException sqlEx2)
            return sqlEx2.Number is 2627 or 2601;

        if (!string.IsNullOrEmpty(indexOrConstraintName))
            return ex.InnerException?.Message?.Contains(indexOrConstraintName, StringComparison.OrdinalIgnoreCase) == true;

        return ex.InnerException?.Message?.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true
            || ex.InnerException?.Message?.Contains("unique", StringComparison.OrdinalIgnoreCase) == true;
    }
}
