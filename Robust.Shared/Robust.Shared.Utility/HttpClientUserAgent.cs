using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Robust.Shared.Utility;

internal static class HttpClientUserAgent
{
	private const string ProductName = "RobustToolbox";

	public static void AddUserAgent(HttpClient client)
	{
		Version version = typeof(HttpClientUserAgent).Assembly.GetName().Version;
		if ((object)version != null)
		{
			client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RobustToolbox", version.ToString()));
		}
	}
}
