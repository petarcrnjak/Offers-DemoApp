using System.Net;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Application.Models;
using Application.Models.Validation;
using Newtonsoft.Json;

namespace Application.Services;

public class OfferItemApiClient : IOfferItemApiClient
{
    private readonly HttpClient _httpClient;

    public OfferItemApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaginatedOffersDto?> GetPaginatedOffersAsync(int pageIndex)
    {
        var requestUri = $"/api/Offers/paginated?pageIndex={pageIndex}";
        var response = await _httpClient.GetAsync(requestUri);

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PaginatedOffersDto>(responseBody);
            return result;
        }

        return null;
    }

    public async Task<OfferDto?> GetOfferByIdAsync(int id)
    {
        try
        {
            var requestUri = $"/api/Offers/{id}";
            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OfferDto>(responseBody);
                return result;
            }

            return null;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<ApiResponse<OfferDto>> UpdateOfferItem(OfferDto offer)
    {
        try
        {
            var requestUri = $"/api/Offers/{offer.Id}";
            var requestData = JsonConvert.SerializeObject(offer);
            var content = new StringContent(requestData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return new ApiResponse<OfferDto>(response.IsSuccessStatusCode, null, JsonConvert.DeserializeObject<OfferDto>(responseBody));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var validationErrorResponse = JsonConvert.DeserializeObject<ApiResponseWrapper<List<ValidationError>>>(responseBody);
                if (validationErrorResponse != null && !validationErrorResponse.Success && validationErrorResponse.Data?.Count != 0)
                {
                    var errorMessage = validationErrorResponse.Data.FirstOrDefault().ErrorMessage;
                    return new ApiResponse<OfferDto>(response.IsSuccessStatusCode, errorMessage);
                }

                return new ApiResponse<OfferDto>(response.IsSuccessStatusCode, "An error occurred while updating the offer.");
            }

            return new ApiResponse<OfferDto>(response.IsSuccessStatusCode, "An error occurred while updating the offer.");
        }
        catch (Exception e)
        {
            return new ApiResponse<OfferDto>(false, "An error occurred while updating the offer.");
        }
    }

    public async Task<bool> DeleteOfferItem(int offerId, int itemId)
    {
        try
        {
            var requestUri = $"/api/Offers/{offerId}/items/{itemId}";
            var response = await _httpClient.DeleteAsync(requestUri);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<OfferDto?> CreateNewOfferAsync()
    {
        try
        {
            var requestUri = "/api/Offers";
            var response = await _httpClient.PostAsync(requestUri, null);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var offer = JsonConvert.DeserializeObject<OfferDto>(responseBody);
                return offer;
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<OfferItemDto>?> GetOfferItemsNames()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/Offers/offerItems");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var offerItemsList = JsonConvert.DeserializeObject<List<OfferItemDto>>(responseBody);
                return offerItemsList;
            }

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<ImportResultDto?> ImportOffersAsync(List<OfferImportDto> offers)
    {
        try
        {
            var requestUri = "/api/Offers/import";
            var requestData = JsonConvert.SerializeObject(offers);
            var content = new StringContent(requestData, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ImportResultDto>(responseBody);
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}