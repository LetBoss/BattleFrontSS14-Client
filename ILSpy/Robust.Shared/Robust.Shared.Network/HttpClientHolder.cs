using System.Net.Http;
using Robust.Shared.Utility;

namespace Robust.Shared.Network;

internal sealed class HttpClientHolder : IHttpClientHolder
{
	public HttpClient Client { get; }

	public HttpClientHolder()
	{
		Client = new HttpClient(HappyEyeballsHttp.CreateHttpHandler());
		HttpClientUserAgent.AddUserAgent(Client);
	}
}
