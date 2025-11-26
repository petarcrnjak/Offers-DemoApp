namespace Application.DataParser;

public interface IUniversalExcelParser
{
    IEnumerable<T> Parse<T>(Stream dataStream) where T : new();
}