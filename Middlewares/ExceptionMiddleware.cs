using System.Net;
using System.Text.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace PatientKiosk.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Always log full exception server-side with TraceIdentifier for correlation
                _logger.LogError(ex, "Unhandled exception occurred while processing request: {Method} {Path} TraceId:{TraceId}", context.Request.Method, context.Request.Path, context.TraceIdentifier);

                // Return standardized error response (detailed diagnostics only in Development)
                await HandleExceptionAsync(context, ex, _env);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment env)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorId = context.TraceIdentifier;
            var payload = new Dictionary<string, object>
            {
                ["error"] = "An unexpected error occurred.",
                ["errorId"] = errorId,
                ["path"] = context.Request.Path.Value ?? string.Empty,
                ["method"] = context.Request.Method,
                ["timestampUtc"] = DateTime.UtcNow
            };

            if (env.IsDevelopment())
            {
                payload["exceptionType"] = exception.GetType().FullName;
                payload["message"] = exception.Message;

                // Extract structured mapping info from exception.Data (set by data-access layer)
                var dataMapping = ExtractMappingFromExceptionData(exception);
                if (dataMapping != null && dataMapping.Count > 0)
                {
                    payload["mapping"] = dataMapping;
                }
                else
                {
                    // Try to parse human-readable messages if no Data present
                    var messageMapping = TryExtractMappingInfo(exception);
                    if (messageMapping != null && messageMapping.Count > 0)
                        payload["mapping"] = messageMapping;
                }

                // Stack frame inspection for file/line/method info (best-effort)
                try
                {
                    var st = new StackTrace(exception, true);
                    var frame = st.GetFrames()?.FirstOrDefault(f => f.GetFileLineNumber() > 0) ?? st.GetFrame(0);
                    if (frame != null)
                    {
                        payload["fileName"] = frame.GetFileName();
                        payload["lineNumber"] = frame.GetFileLineNumber();
                        payload["methodName"] = frame.GetMethod()?.Name;
                    }

                    payload["stackTrace"] = exception.ToString();
                }
                catch
                {
                    // Fallback: include basic exception.ToString()
                    payload["stackTrace"] = exception.ToString();
                }
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(payload, options);
            return context.Response.WriteAsync(json);
        }

        // Read mapping metadata if the data-access layer attached it to exception.Data
        private static Dictionary<string, object>? ExtractMappingFromExceptionData(Exception exception)
        {
            try
            {
                if (exception.Data == null || exception.Data.Count == 0)
                    return null;

                var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                foreach (var keyObj in exception.Data.Keys)
                {
                    if (keyObj is not string key) continue;
                    var val = exception.Data[keyObj] ?? string.Empty;

                    switch (key.ToLowerInvariant())
                    {
                        case "model":
                        case "modeltype":
                        case "typename":
                        case "targettype":
                            result["model"] = val;
                            break;
                        case "propertyname":
                        case "membername":
                        case "property":
                            result["property"] = val;
                            break;
                        case "expectedtype":
                        case "targettypefullname":
                        case "expected":
                            result["expectedType"] = val;
                            break;
                        case "actualvalue":
                        case "value":
                        case "actual":
                            result["actualValue"] = val;
                            break;
                        default:
                            // include any other useful info
                            if (!result.ContainsKey(key))
                                result[key] = val;
                            break;
                    }
                }

                return result.Count > 0 ? result : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Attempts to extract model mapping/conversion diagnostics for known exception types by parsing message text.
        /// Best-effort only; does not throw.
        /// </summary>
        private static Dictionary<string, object>? TryExtractMappingInfo(Exception exception)
        {
            try
            {
                // Check Json exceptions (System.Text.Json)
                if (exception is System.Text.Json.JsonException sysJsonEx)
                {
                    var result = new Dictionary<string, object>();
                    var path = sysJsonEx.Path;
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        var prop = path.Split('.', '[', ']').LastOrDefault(p => !string.IsNullOrWhiteSpace(p));
                        if (!string.IsNullOrWhiteSpace(prop))
                            result["property"] = prop;

                        result["jsonPath"] = path;
                    }

                    result["bytePositionInLine"] = sysJsonEx.BytePositionInLine;
                    return result.Count > 0 ? result : null;
                }

                // Check for Newtonsoft.Json exceptions by type name (best-effort)
                var exTypeName = exception.GetType().FullName ?? string.Empty;
                if (exTypeName.StartsWith("Newtonsoft.Json.", StringComparison.Ordinal))
                {
                    var result = new Dictionary<string, object>();
                    var pathProp = exception.GetType().GetProperty("Path");
                    var pathVal = pathProp?.GetValue(exception) as string;
                    if (!string.IsNullOrWhiteSpace(pathVal))
                    {
                        var prop = pathVal.Split('.', '[', ']').LastOrDefault(p => !string.IsNullOrWhiteSpace(p));
                        if (!string.IsNullOrWhiteSpace(prop))
                            result["property"] = prop;
                        result["jsonPath"] = pathVal;
                    }

                    var lineProp = exception.GetType().GetProperty("LineNumber");
                    var posProp = exception.GetType().GetProperty("LinePosition");
                    if (lineProp?.GetValue(exception) is int ln)
                        result["lineNumber"] = ln;
                    if (posProp?.GetValue(exception) is int lp)
                        result["linePosition"] = lp;

                    return result.Count > 0 ? result : null;
                }

                // Handle InvalidCastException, OverflowException, FormatException and others arising during mapping
                if (exception is InvalidCastException || exception is OverflowException || exception is FormatException)
                {
                    var message = exception.Message ?? string.Empty;
                    var result = ParseMappingMessage(message);

                    // Also inspect inner exception messages for more detail
                    if ((result == null || result.Count == 0) && exception.InnerException != null)
                        result = ParseMappingMessage(exception.InnerException.Message ?? string.Empty);

                    return result;
                }

                // For aggregate exceptions, inspect inner exceptions
                if (exception is AggregateException aggEx && aggEx.InnerExceptions != null && aggEx.InnerExceptions.Count > 0)
                {
                    foreach (var inner in aggEx.InnerExceptions)
                    {
                        var r = TryExtractMappingInfo(inner);
                        if (r != null && r.Count > 0) return r;
                    }
                }

                // Some mapping libraries attach contextual data in Data dictionary (covered earlier)
                return null;
            }
            catch
            {
                // Never propagate parsing errors
                return null;
            }
        }

        /// <summary>
        /// Improved heuristic parsing of exception message to extract mapping information.
        /// </summary>
        private static Dictionary<string, object>? ParseMappingMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return null;

            var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            // Pattern: property 'X' on type 'Y'
            var propTypePattern = new Regex(@"property\s+['""](?<property>[^'""]+)['""]\s+on\s+(?:type|class)\s+['""](?<model>[^'""]+)['""]", RegexOptions.IgnoreCase);
            var m = propTypePattern.Match(message);
            if (m.Success)
            {
                if (!string.IsNullOrEmpty(m.Groups["property"].Value))
                    result["property"] = m.Groups["property"].Value;
                if (!string.IsNullOrEmpty(m.Groups["model"].Value))
                    result["model"] = m.Groups["model"].Value;
            }

            // Pattern: Path 'X' (common in JSON converters)
            var pathPattern = new Regex(@"Path\s+['""](?<path>[^'""]+)['""]", RegexOptions.IgnoreCase);
            m = pathPattern.Match(message);
            if (m.Success)
            {
                var path = m.Groups["path"].Value;
                var prop = path.Split('.', '[', ']').LastOrDefault(p => !string.IsNullOrWhiteSpace(p));
                if (!string.IsNullOrWhiteSpace(prop))
                    result["property"] = prop;
                result["jsonPath"] = path;
            }

            // Pattern: Cannot convert value 'X' to type 'Y'
            var convertPattern1 = new Regex(@"convert(?:ing)?\s+(?:value\s+)?['""](?<value>[^'""]+)['""]\s+to\s+type\s+['""]?(?<type>[^'""]+)['""]?", RegexOptions.IgnoreCase);
            m = convertPattern1.Match(message);
            if (m.Success)
            {
                if (!string.IsNullOrEmpty(m.Groups["value"].Value))
                    result["actualValue"] = m.Groups["value"].Value;
                if (!string.IsNullOrEmpty(m.Groups["type"].Value))
                    result["expectedType"] = m.Groups["type"].Value;
            }

            // Pattern: Could not convert string to integer: '12345'.
            var convertPattern2 = new Regex(@"Could not convert (?:string|value) to (?<type>[^\:]+)\:\s*['""]?(?<value>[^'""]+)['""]?", RegexOptions.IgnoreCase);
            m = convertPattern2.Match(message);
            if (m.Success)
            {
                var type = m.Groups["type"].Value?.Trim();
                var val = m.Groups["value"].Value;
                if (!string.IsNullOrWhiteSpace(type)) result["expectedType"] = type;
                if (!string.IsNullOrWhiteSpace(val)) result["actualValue"] = val;
            }

            // Improved overflow pattern: allow optional 'an' or 'a' before type
            var overflowPattern = new Regex(@"too\s+large\s+or\s+too\s+small\s+for\s+(?:an\s+|a\s+)?(?<type>[\w\.]+)", RegexOptions.IgnoreCase);
            m = overflowPattern.Match(message);
            if (m.Success)
            {
                result["expectedType"] = m.Groups["type"].Value;
            }

            // Input string format hint
            if (message.IndexOf("Input string was not in a correct format", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                result["errorKind"] = "FormatException";
            }

            return result.Count > 0 ? result : null;
        }
    }
}