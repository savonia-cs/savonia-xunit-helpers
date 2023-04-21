using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using System.Net.Http.Headers;
using Xunit;

namespace Savonia.xUnit.Helpers;

// from https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/test/integration-tests/samples/3.x/IntegrationTestsSample/tests/RazorPagesProject.Tests/Helpers
/// <summary>
/// Helper methods for html.
/// </summary>
public static class HtmlHelpers
{
    /// <summary>
    /// Gets a html document (<see cref="IHtmlDocument"/>) from a <see cref="HttpResponseMessage"/>
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var document = await BrowsingContext.New()
            .OpenAsync(ResponseFactory, CancellationToken.None);
        return (IHtmlDocument)document;

        void ResponseFactory(VirtualResponse htmlResponse)
        {
            htmlResponse
                .Address(response.RequestMessage?.RequestUri)
                .Status(response.StatusCode);

            MapHeaders(response.Headers);
            MapHeaders(response.Content.Headers);

            htmlResponse.Content(content);

            void MapHeaders(HttpHeaders headers)
            {
                foreach (var header in headers)
                {
                    foreach (var value in header.Value)
                    {
                        htmlResponse.Header(header.Key, value);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get html form element. Asserts that found element is <see cref="IHtmlFormElement"/>.
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public static IHtmlFormElement GetForm(this IHtmlDocument document)
    {
        var element = document.QuerySelector("form");
        var typedElement = Assert.IsAssignableFrom<IHtmlFormElement>(element);

        return typedElement;
    }    

    /// <summary>
    /// Get single html element from form. Asserts that found element is <see cref="IHtmlElement"/>.
    /// </summary>
    /// <param name="form"></param>
    /// <param name="querySelector"></param>
    /// <returns></returns>
    public static IHtmlElement GetElement(this IHtmlFormElement form, string querySelector)
    {
        var queryResults = form.QuerySelectorAll(querySelector);
        var element = Assert.Single(queryResults);
        var typedElement = Assert.IsAssignableFrom<IHtmlElement>(element);

        return typedElement;
    }    

    /// <summary>
    /// Get single submit button from form.
    /// The button can be input element with type="submit" or button element with empty type or type="submit".
    /// Button element without type is by default a submit button.
    /// Asserts that only one element is found and that it is assignable from <see cref="IHtmlElement"/>
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    public static IHtmlElement GetSubmitButton(this IHtmlFormElement form)
    {
        object? submitElement = null;
        var inputs = form.QuerySelectorAll("[type=submit]");
        if (inputs.Any())
        {
            submitElement = Assert.Single(inputs);
        }
        else
        {
            var buttons = form.QuerySelectorAll("button").Where(m => m.Attributes["type"]?.Value == null || m.Attributes["type"]?.Value.ToLower() == "submit");
            submitElement = Assert.Single(buttons);
        }
        var submitButton = Assert.IsAssignableFrom<IHtmlElement>(submitElement);
        return submitButton;
    }    

}
