using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;

namespace Application.DataParser;

public class UniversalExcelParser : IUniversalExcelParser
{
    public IEnumerable<T> Parse<T>(Stream dataStream) where T : new()
    {
        using var workbook = new XLWorkbook(dataStream);
        var worksheet = workbook.Worksheet(1);
        var headerRow = worksheet.Row(1);
        
        // Get property mappings
        var properties = typeof(T).GetProperties();
        var columnMap = new Dictionary<PropertyInfo, int>();
        
        foreach (var cell in headerRow.CellsUsed())
        {
            var columnName = cell.GetString().Trim();
            var property = properties.FirstOrDefault(p => 
                p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
            
            if (property != null)
            {
                columnMap[property] = cell.Address.ColumnNumber;
            }
        }

        // Check if T has a RowNumber property
        var rowNumberProperty = typeof(T).GetProperty("RowNumber");

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            var item = new T();
            
            // Set RowNumber if property exists
            if (rowNumberProperty != null && rowNumberProperty.CanWrite)
            {
                rowNumberProperty.SetValue(item, row.RowNumber()-1);
            }

            // Map other properties
            foreach (var mapping in columnMap)
            {
                var cell = row.Cell(mapping.Value);
                if (!cell.IsEmpty())
                {
                    try 
                    {
                        var value = GetCellValue(cell, mapping.Key.PropertyType);
                        mapping.Key.SetValue(item, value);
                    }
                    catch (Exception ex)
                    {
                        throw new FormatException(
                            $"Error in row {row.RowNumber()}, column {cell.Address.ColumnLetter}: {ex.Message}");
                    }
                }
            }

            yield return item;
        }
    }

    private static object? GetCellValue(IXLCell cell, Type targetType)
    {
        if (cell.IsEmpty()) 
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        try
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            
            // Handle DateTime separately
            if (underlyingType == typeof(DateTime))
            {
                return ParseDateTimeCell(cell);
            }
            
            if (underlyingType == typeof(string)) 
                return cell.GetString();
            
            if (underlyingType == typeof(bool)) 
                return cell.GetBoolean();
            
            if (underlyingType.IsEnum) 
                return Enum.Parse(underlyingType, cell.GetString(), true);
            
            if (underlyingType == typeof(int))
                return (int)cell.GetDouble();
            
            if (underlyingType == typeof(decimal))
                return cell.GetValue<decimal>();
            
            if (underlyingType == typeof(double))
                return cell.GetValue<double>();

            return Convert.ChangeType(cell.Value, underlyingType);
        }
        catch (Exception ex)
        {
            throw new FormatException(
                $"Could not convert cell value '{cell.Value}' to {targetType.Name}", ex);
        }
    }

    private static DateTime ParseDateTimeCell(IXLCell cell)
    {
        // 1. First try ClosedXML's built-in DateTime conversion
        if (cell.TryGetValue(out DateTime dateValue))
            return dateValue;

        // 2. Try parsing common string formats
        if (cell.DataType == XLDataType.Text)
        {
            string[] dateFormats = {
                "yyyy-MM-ddTHH:mm:ss",    // ISO 8601
                "yyyy-MM-dd",             // Date only
                "MM/dd/yyyy",             // US format
                "dd/MM/yyyy",             // European format
                "yyyyMMdd",               // Compact format
                "yyyy-MM-dd HH:mm:ss",    // With space separator
                "M/d/yyyy h:mm:ss tt",    // US with AM/PM
                "yyyy-MM-ddTHH:mm:ss.fff" // ISO with milliseconds
            };

            if (DateTime.TryParseExact(cell.GetString(), 
                    dateFormats, 
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, 
                    out dateValue))
            {
                return dateValue;
            }
        }

        // 3. Fallback to OLE Automation date
        if (cell.DataType == XLDataType.Number)
            return DateTime.FromOADate(cell.GetDouble());

        throw new FormatException($"Could not parse DateTime value from cell {cell.Address}");
    }
}