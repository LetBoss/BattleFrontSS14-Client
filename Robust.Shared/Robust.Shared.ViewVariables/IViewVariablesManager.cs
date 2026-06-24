using System.Collections.Generic;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;
using Robust.Shared.Player;

namespace Robust.Shared.ViewVariables;

[NotContentImplementable]
public interface IViewVariablesManager
{
	void RegisterDomain(string domain, DomainResolveObject resolveObject, DomainListPaths list);

	bool UnregisterDomain(string domain);

	ViewVariablesTypeHandler<T> GetTypeHandler<T>();

	ViewVariablesPath? ResolvePath(string path);

	object? ReadPath(string path);

	string? ReadPathSerialized(string path);

	void WritePath(string path, string value);

	object? InvokePath(string path, string arguments);

	IEnumerable<string> ListPath(string path, VVListPathOptions options);

	Task<string?> ReadRemotePath(string path, ICommonSession? session = null);

	Task WriteRemotePath(string path, string value, ICommonSession? session = null);

	Task<string?> InvokeRemotePath(string path, string arguments, ICommonSession? session = null);

	Task<IEnumerable<string>> ListRemotePath(string path, VVListPathOptions options, ICommonSession? session = null);
}
