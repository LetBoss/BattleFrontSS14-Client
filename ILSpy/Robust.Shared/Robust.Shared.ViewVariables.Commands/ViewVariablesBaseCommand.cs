using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Robust.Shared.ViewVariables.Commands;

public abstract class ViewVariablesBaseCommand : LocalizedCommands
{
	[Dependency]
	protected readonly INetManager _netMan;

	[Dependency]
	protected readonly IViewVariablesManager _vvm;

	public override async ValueTask<CompletionResult> GetCompletionAsync(IConsoleShell shell, string[] args, string argStr, CancellationToken cancel)
	{
		int num = args.Length;
		if ((num > 1 || num == 0) ? true : false)
		{
			return CompletionResult.Empty;
		}
		string text = args[0];
		if (_netMan.IsClient)
		{
			if (text.StartsWith("/c"))
			{
				IViewVariablesManager vvm = _vvm;
				string text2 = text;
				return CompletionResult.FromOptions(from p in vvm.ListPath(text2.Substring(2, text2.Length - 2), new VVListPathOptions())
					select new CompletionOption("/c" + p, null, CompletionOptionFlags.PartialCompletion));
			}
			return CompletionResult.FromOptions((await _vvm.ListRemotePath(text, new VVListPathOptions())).Select((string p) => new CompletionOption(p, null, CompletionOptionFlags.PartialCompletion)).Append(new CompletionOption("/c", "Client-side paths", CompletionOptionFlags.PartialCompletion)));
		}
		return CompletionResult.FromOptions(from p in _vvm.ListPath(text, new VVListPathOptions())
			select new CompletionOption(p, null, CompletionOptionFlags.PartialCompletion));
	}
}
