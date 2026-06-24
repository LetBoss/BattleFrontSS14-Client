using Robust.Shared.GameObjects;
using Robust.Shared.Toolshed.Errors;

namespace Robust.Shared.Toolshed.Commands.Players;

[ToolshedCommand]
internal sealed class SelfCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public EntityUid Self(IInvocationContext ctx)
	{
		if (ctx.Session == null)
		{
			ctx.ReportError(new NotForServerConsoleError());
			return default(EntityUid);
		}
		EntityUid? attachedEntity = ctx.Session.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			return attachedEntity.GetValueOrDefault();
		}
		ctx.ReportError(new SessionHasNoEntityError(ctx.Session));
		return default(EntityUid);
	}
}
