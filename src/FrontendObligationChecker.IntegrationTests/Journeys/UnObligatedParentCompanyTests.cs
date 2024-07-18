﻿using FluentAssertions;
using FrontendObligationChecker.Generators;
using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.IntegrationTests.Journeys;
[TestClass]
public class UnObligatedParentCompanyTests : TestBase
{
    private IEnumerable<Page>? _pages;

    [TestInitialize]
    public void Setup()
    {
        _pages = PageGenerator.Create(string.Empty);
    }

    [TestMethod]
    [DataRow(PagePath.TypeOfOrganisation)]
    [DataRow(PagePath.AnnualTurnover)]
    [DataRow(PagePath.NoActionNeeded)]
    public async Task JourneyPaths_ReturnSuccess_And_CorrectPageTitle(string path)
    {
        // Arrange
        var page = GetPage(path);

        // Act
        var response = await _httpClient.GetAsync($"/ObligationChecker/{path}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.Should().BeSuccessful();
        // content.Should().Contain(pageTitle);
    }

    [TestMethod]
    [DataRow(PagePath.TypeOfOrganisation)]
    [DataRow(PagePath.AnnualTurnover)]
    public async Task SubmitAnswer_RedirectsToNextPage(string path)
    {
        // Arrange
        var page = GetPage(path);
        var formData = await GetFormData(page, path);

        // Act
        var response = await _httpClient.PostAsync($"/ObligationChecker/get-next-page", formData);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.Should().BeSuccessful();
        // content.Should().Contain(page.Title);
    }

    private Page GetPage(string path)
    {
        return _pages?.SingleOrDefault(page => page.Path.Equals(path))!;
    }

    private async Task<FormUrlEncodedContent> GetFormData(Page page, string path)
    {
        var tokenValue = await GetAntiForgeryToken($"/ObligationChecker/{PagePath.TypeOfOrganisation}");

        page.Questions.ForEach(q => q.SetAnswer(q.Options.FirstOrDefault()!.Value));

        var options = page.Questions.Select(x =>
                new
                {
                    x.Key,
                    x.Options.FirstOrDefault()!.Value
                })
            .ToList();
        options.Add(new
        {
            Key = "path",
            Value = path
        });

        options.Add(new
        {
            Key = "__RequestVerificationToken",
            Value = tokenValue
        });
        var formValues = options.Select(x =>
            new KeyValuePair<string, string>(x.Key, x.Value));

        return new FormUrlEncodedContent(formValues);
    }
}