using System.Net.Http;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Network;

[NotContentImplementable]
public interface IHttpClientHolder
{
	HttpClient Client { get; }
}
