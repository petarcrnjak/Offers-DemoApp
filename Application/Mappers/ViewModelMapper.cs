using Application.DTOs;
using Application.Validations.Models;

namespace Application.Mappers;

public static class ViewModelMapper
{
    public static ImportResult MapToViewModel(ImportResultDto apiResult)
    {
        return new ImportResult
        {
            ProcessedDate = apiResult.ProcessedDate,
            SuccessCount = apiResult.SuccessCount,
            Errors = apiResult.Errors.Select(e => new RowError
            {
                RowNumber = e.RowNumber,
                Errors = e.Errors.ToList()
            }).ToList()
        };
    }
}