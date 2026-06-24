using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Commands.Vfs;

[ToolshedCommand]
internal sealed class LsCommand : VfsCommand
{
	[CommandImplementation("here")]
	public IEnumerable<ResPath> LsHere(IInvocationContext ctx)
	{
		ResPath curPath = CurrentPath(ctx);
		return from x in Resources.ContentGetDirectoryEntries(curPath)
			select curPath / x;
	}

	[CommandImplementation("in")]
	public IEnumerable<ResPath> LsIn(IInvocationContext ctx, ResPath @in)
	{
		ResPath curPath = CurrentPath(ctx);
		if (@in.IsRooted)
		{
			curPath = @in;
		}
		else
		{
			curPath /= @in;
		}
		return from x in Resources.ContentGetDirectoryEntries(curPath)
			select curPath / x;
	}
}
