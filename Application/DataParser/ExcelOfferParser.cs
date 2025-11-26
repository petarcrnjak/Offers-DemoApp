using Application.DTOs;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace Application.DataParser;

/*public class ExcelOfferImportDtoParser : IDataParser<OfferImportDto>
{
    // public List<OfferImportDto> Parse(IFormFile excelFile)
    // {
    //     var offers = new List<OfferImportDto>();
    //
    //     using var stream = new MemoryStream();
    //     excelFile.CopyTo(stream);
    //
    //     using var workbook = new XLWorkbook(stream);
    //     var worksheet = workbook.Worksheet(1);
    //
    //     foreach (var row in worksheet.RowsUsed().Skip(1))
    //         offers.Add(new OfferImportDto
    //         {
    //             RowNumber = row.RowNumber(),
    //             OfferId = GetCellValue<int>(row.Cell(1)),
    //             Date = ParseExcelDate(row.Cell(2), row.RowNumber()),
    //             ArticleId = GetCellValue<int?>(row.Cell(3)),
    //             Article = GetCellValue<string>(row.Cell(4)),
    //             UnitPrice = GetCellValue<decimal?>(row.Cell(5)),
    //             Quantity = GetCellValue<int?>(row.Cell(6))
    //         });
    //
    //
    //     return offers;
    // }
    
    public IEnumerable<OfferImportDto> Parse(Stream dataStream)
    {
        var offers = new List<OfferImportDto>();

        using var workbook = new XLWorkbook(dataStream);
        var worksheet = workbook.Worksheet(1); // Assuming first worksheet

        foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header
            offers.Add(new OfferImportDto
            {
                RowNumber = row.RowNumber(),
                OfferId = GetCellValue<int>(row.Cell(1)),
                Date = ParseExcelDate(row.Cell(2), row.RowNumber()),
                ArticleId = GetCellValue<int?>(row.Cell(3)),
                Article = GetCellValue<string>(row.Cell(4)),
                UnitPrice = GetCellValue<decimal?>(row.Cell(5)),
                Quantity = GetCellValue<int?>(row.Cell(6))
            });

        return offers;
    }

    private static T GetCellValue<T>(IXLCell cell)
    {
        try
        {
            return cell.GetValue<T>();
        }
        catch
        {
            throw new FormatException($"Invalid {typeof(T).Name} value in row {cell.Address.RowNumber}, column {cell.Address.ColumnLetter}");
        }
    }

    private static DateTime ParseExcelDate(IXLCell cell, int rowNumber)
    {
        if (DateTime.TryParse(cell.GetString(), out var parsedDate))
            return parsedDate;

        throw new FormatException($"Invalidan format datuma u  date format in redu {rowNumber}");
    }
}*/