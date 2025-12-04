using Application.DataParser;
using Application.DTOs;
using Application.Interfaces;
using Application.Mappers;
using Microsoft.AspNetCore.Mvc;
using Ponude.Models;

namespace Ponude.Controllers;

public class OffersController : Controller
{
    private readonly IUniversalExcelParser _excelParser;
    private readonly IOfferItemApiClient _offerItemClient;

    public OffersController(IOfferItemApiClient offerItemClient, IUniversalExcelParser excelParser)
    {
        _offerItemClient = offerItemClient;
        _excelParser = excelParser;
    }

    public async Task<IActionResult> Index(int pageIndex = 1)
    {
        var response = await _offerItemClient.GetPaginatedOffersAsync(pageIndex);
        if (response == null || response.Offers == null || response.Offers.Count == 0)
        {
            var emptyModel = new PaginatedOffersViewModel
            {
                Offers = [],
                TotalPages = 1,
                CurrentPage = pageIndex
            };
            return View(emptyModel);
        }

        var model = new PaginatedOffersViewModel
        {
            Offers = response.Offers,
            TotalPages = response.TotalPages,
            CurrentPage = pageIndex
        };

        return View(model);
    }

    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (id.HasValue && id.Value != 0)
        {
            var offer = await _offerItemClient.GetOfferByIdAsync(id.Value);
            if (offer == null)
            {
                ViewBag.ErrorMessage = "Ponuda nije dostupna.";
                return View(new OfferDto());
            }

            ViewBag.TotalAmount = offer.TotalAmount;
            return View(offer);
        }

        // Creating new offer
        var newOffer = await _offerItemClient.CreateNewOfferAsync();
        if (newOffer == null)
        {
            TempData["ErrorMessage"] = "Greška kod kreiranja narudžbe, pokušajte ponovo.";
            return View(new OfferDto());
        }

        return View(new OfferDto
        {
            Id = newOffer.Id,
            Date = DateTime.Now,
            OfferItems = []
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetArticleNames()
    {
        try
        {
            var articles = await _offerItemClient.GetOfferItemsNames();
            if (articles == null || articles.Count == 0)
                return Json(new { warning = "No articles available" });

            return Ok(articles.Select(a => new
            {
                id = a.Id,
                article = a.Article,
                unitPrice = a.UnitPrice
            }));
        }
        catch (Exception ex)
        {
            return Json(new { error = "Failed to load articles" });
        }

        return View(new OfferDto());
    }

    public async Task<IActionResult> SaveOffer(OfferDto offer)
    {
        if (offer.OfferItems is null or { Count: 0 })
            return View("CreateEdit", offer);

        var updatedOfferResponse = await _offerItemClient.UpdateOfferItem(offer);

        if (!updatedOfferResponse.Success)
        {
            TempData["ErrorMessage"] = (updatedOfferResponse.ErrorMessage ?? "Unknown error occurred.").Trim();
            return RedirectToAction("CreateEdit", new { id = offer.Id });
        }

        TempData["SuccessMessage"] = "Ponuda je uspješno ažurirana.";
        return RedirectToAction("CreateEdit", new { id = offer.Id });

        return View(new OfferDto());
    }

    public async Task<IActionResult> DeleteItem(int offerId, int itemId)
    {
        var success = await _offerItemClient.DeleteOfferItem(offerId, itemId);
        if (!success)
            TempData["ErrorMessage"] = "Failed to delete item.";

        return RedirectToAction("CreateEdit", new { id = offerId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportData(ImportViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Excel", model);

        if (model.ExcelFile.Length == 0)
        {
            ModelState.AddModelError("ExcelFile", "Please select an Excel file");
            return View(model);
        }

        if (model.ExcelFile is { Length: > 0 })
            try
            {
                using var stream = model.ExcelFile.OpenReadStream();
                var offers = _excelParser.Parse<OfferImportDto>(stream);

                var apiResult = await _offerItemClient.ImportOffersAsync(offers.ToList());

                if (apiResult == null)
                    throw new Exception("API request failed");

                model.ImportResult = ViewModelMapper.MapToViewModel(apiResult);

                if (apiResult.Errors.Count != 0)
                    TempData["ErrorMessage"] = $"Import completed with {apiResult.Errors.Count} errors";
                else
                    TempData["SuccessMessage"] = $"Successfully imported {apiResult.SuccessCount} offers";

                return View("ImportData", model);
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = $"Import failed: {e.Message}";
                return View("ImportData", model);
            }

        return View("ImportData", model);
    }

    [HttpGet]
    public IActionResult ImportData()
    {
        return View();
    }
}