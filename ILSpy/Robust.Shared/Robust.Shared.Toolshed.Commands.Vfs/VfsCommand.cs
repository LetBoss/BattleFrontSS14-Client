using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Commands.Vfs;

public abstract class VfsCommand : ToolshedCommand
{
	[Dependency]
	protected readonly IResourceManager Resources;

	public const string UserVfsLocVariableName = "user_vfs_loc";

	protected ResPath CurrentPath(IInvocationContext ctx)
	{
		return ((ResPath?)ctx.ReadVar("user_vfs_loc")) ?? ResPath.Root;
	}

	protected void SetPath(IInvocationContext ctx, ResPath path)
	{
		ctx.WriteVar("user_vfs_loc", path.Clean());
	}
}
