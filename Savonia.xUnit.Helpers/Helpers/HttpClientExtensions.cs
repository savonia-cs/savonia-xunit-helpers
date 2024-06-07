using AngleSharp.Html.Dom;
using Xunit;

namespace Savonia.xUnit.Helpers;


// from https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/test/integration-tests/samples/3.x/IntegrationTestsSample/tests/RazorPagesProject.Tests/Helpers
/// <summary>
/// Helper methods for <see cref="HttpClient"/> for testing Razor pages.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Submits a form via <paramref name="submitButton"/> click with empty form data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="form"></param>
    /// <param name="submitButton"></param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> SendAsync(
        this HttpClient client,
        IHtmlFormElement form,
        IHtmlElement submitButton)
    {
        return client.SendAsync(form, submitButton, new Dictionary<string, string>());
    }

    /// <summary>
    /// Submits a form via <paramref name="submitButton"/> click with <paramref name="formValues"/> form data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="form"></param>
    /// <param name="submitButton"></param>
    /// <param name="formValues"></param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> SendAsync(
        this HttpClient client,
        IHtmlFormElement form,
        IHtmlElement submitButton,
        IEnumerable<KeyValuePair<string, string>> formValues)
    {
        foreach (var kvp in formValues)
        {
            var element = Assert.IsAssignableFrom<IHtmlInputElement>(form[kvp.Key]);
            element.Value = kvp.Value;
        }

        var submitRequest = form.GetSubmission(submitButton);
        if (null == submitRequest)
        {
            throw new NullReferenceException($"Could not create submit request on form {form.BaseUri} with submit button {submitButton.OuterHtml}. Check that proper values in expected format are provided for input elements and the input elements are configured for the provided values. Dates and times are usually the problem.");
        }
        var target = (Uri)submitRequest.Target;
        if (submitButton.HasAttribute("formaction"))
        {
            var formaction = submitButton.GetAttribute("formaction");
            if (false == string.IsNullOrEmpty(formaction))
            {
                target = new Uri(formaction, UriKind.Relative);
            }
        }
        var submission = new HttpRequestMessage(new HttpMethod(submitRequest.Method.ToString()), target)
        {
            Content = new StreamContent(submitRequest.Body)
        };

        foreach (var header in submitRequest.Headers)
        {
            submission.Headers.TryAddWithoutValidation(header.Key, header.Value);
            submission.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return client.SendAsync(submission);
    }
}
