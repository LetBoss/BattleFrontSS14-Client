using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Commands.Vfs;

[ToolshedCommand]
internal sealed class CdCommand : VfsCommand
{
	[CommandImplementation(null)]
	public void Cd(IInvocationContext ctx, ResPath path)
	{
		ResPath path2 = CurrentPath(ctx);
		if (path.IsRooted)
		{
			path2 = path;
		}
		else
		{
			path2 /= path;
		}
		SetPath(ctx, path2);
	}
}
