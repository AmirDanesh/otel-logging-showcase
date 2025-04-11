using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] SensitiveKeys = { "password", "pass", "pwd", "token", "access_token", "authorization" };

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var activity = Activity.Current;

        // === Request ===
        context.Request.EnableBuffering();

        var requestBody = string.Empty;
        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        var maskedRequestBody = MaskSensitiveJson(requestBody);

        var headers = context.Request.Headers
            .ToDictionary(h => h.Key, h => SensitiveKeys.Contains(h.Key.ToLower()) ? "***" : h.Value.ToString());

        var headerString = string.Join(" | ", headers.Select(h => $"{h.Key}: {h.Value}"));
        var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
        var clientIp = context.Connection?.RemoteIpAddress?.MapToIPv4().ToString();
        var serverIp = context.Connection?.LocalIpAddress?.MapToIPv4().ToString();

        activity?.SetTag("http.request.ip", clientIp);
        activity?.SetTag("http.server.ip", serverIp);
        activity?.SetTag("http.request.body", maskedRequestBody);
        activity?.SetTag("http.request.headers", headerString);
        activity?.SetTag("http.request.query", queryString);


        // === Response ===
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        await _next(context);

        responseBodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
        responseBodyStream.Seek(0, SeekOrigin.Begin);

        var maskedResponseBody = MaskSensitiveJson(responseBody);

        var responseHeaders = context.Response.Headers
            .ToDictionary(h => h.Key, h => SensitiveKeys.Contains(h.Key.ToLower()) ? "***" : h.Value.ToString());

        var responseHeaderString = string.Join(" | ", responseHeaders.Select(h => $"{h.Key}: {h.Value}"));

        activity?.SetTag("http.response.headers", responseHeaderString);
        activity?.SetTag("http.response.body", maskedResponseBody);

        await responseBodyStream.CopyToAsync(originalBodyStream);
    }

    private static string MaskSensitiveJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || (!json.TrimStart().StartsWith("{") && !json.TrimStart().StartsWith("[")))
            return json;

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = MaskJsonElement(doc.RootElement);
            return JsonSerializer.Serialize(root);
        }
        catch
        {
            return json;
        }
    }

    private static object MaskJsonElement(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in element.EnumerateObject())
            {
                var key = prop.Name;
                var value = prop.Value;
                dict[key] = SensitiveKeys.Contains(key.ToLower()) ? "***" : MaskJsonElement(value);
            }
            return dict;
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            return element.EnumerateArray().Select(MaskJsonElement).ToList();
        }
        else
        {
            return element.ToString();
        }
    }
}
