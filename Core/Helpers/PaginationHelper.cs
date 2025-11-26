namespace Core.Helpers;

public static class PaginationHelper
{
    public static int CalculateTotalPages(int totalItems, int pageSize)
    {
        return (int)Math.Ceiling((double)totalItems / pageSize);
    }
}