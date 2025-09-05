using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Box.V2.Exceptions;
using System.Net;
using System.Text.RegularExpressions;

namespace Apps.Box;

public class BoxInvocable : BaseInvocable
{
    protected IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    protected BlackbirdBoxClient Client { get; }

    private static readonly int MaxAttempts = 4;
    private static readonly TimeSpan BaseDelay = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(30);
    private static readonly Random _rng = new Random();

    public BoxInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());

    }

    protected async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (BoxAPIException ex)
        {
            throw new PluginApplicationException(ex.ErrorDescription);
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
    }

    protected async Task<T> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> action)
    {
        var delay = BaseDelay;

        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await action();
            }
            catch (BoxAPIException ex) when (IsRetryableStatus(ex.StatusCode))
            {
                var retryAfter = TryGetRetryAfterSeconds(ex);
                var wait = retryAfter ?? WithJitter(delay);

                if (attempt >= MaxAttempts)
                    throw new PluginApplicationException($"Box API error {(int)ex.StatusCode}: {ex.Message}");

                await Task.Delay(wait);
                delay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds * 2, MaxDelay.TotalMilliseconds));
                continue;
            }
            catch (TaskCanceledException)
            {
                if (attempt >= MaxAttempts) throw;
                await Task.Delay(WithJitter(delay));
                delay = TimeSpan.FromMilliseconds(Math.Min(delay.TotalMilliseconds * 2, MaxDelay.TotalMilliseconds));
                continue;
            }
        }
    }



    //Helpers for checking (testing)

    private static bool IsRetryableStatus(HttpStatusCode code)
            => code == HttpStatusCode.ServiceUnavailable
            || code == HttpStatusCode.InternalServerError
            || code == HttpStatusCode.BadGateway
            || code == HttpStatusCode.GatewayTimeout
            || (int)code == 429;

    private static TimeSpan WithJitter(TimeSpan delay)
    {
        var factor = 0.8 + 0.4 * _rng.NextDouble();
        var ms = Math.Min(MaxDelay.TotalMilliseconds, delay.TotalMilliseconds * factor);
        return TimeSpan.FromMilliseconds(ms);
    }
    private static TimeSpan? TryGetRetryAfterSeconds(BoxAPIException ex)
    {
        var headers = ex.GetType().GetProperty("ResponseHeaders")?.GetValue(ex) as System.Collections.Generic.IDictionary<string, string>
                   ?? ex.GetType().GetProperty("Headers")?.GetValue(ex) as System.Collections.Generic.IDictionary<string, string>;
        if (headers != null && headers.TryGetValue("Retry-After", out var val) && int.TryParse(val, out var secs))
            return TimeSpan.FromSeconds(secs);

        var m = Regex.Match(ex.Message ?? "", @"Retry-After:\s*(\d+)", RegexOptions.IgnoreCase);
        if (m.Success && int.TryParse(m.Groups[1].Value, out secs))
            return TimeSpan.FromSeconds(secs);

        return null;
    }
}