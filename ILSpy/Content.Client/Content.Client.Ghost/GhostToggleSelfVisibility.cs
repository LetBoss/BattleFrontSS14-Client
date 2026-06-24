using Content.Shared.Ghost;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Ghost;

public sealed class GhostToggleSelfVisibility : LocalizedEntityCommands
{
	[Dependency]
	private SpriteSystem _sprite;

	public override string Command => "toggleselfghost";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		ICommonSession player = shell.Player;
		EntityUid? val = ((player != null) ? player.AttachedEntity : ((EntityUid?)null));
		if (val.HasValue)
		{
			SpriteComponent val2 = default(SpriteComponent);
			if (!base.EntityManager.HasComponent<GhostComponent>(val))
			{
				shell.WriteError(((LocalizedCommands)this).Loc.GetString("cmd-toggleselfghost-must-be-ghost"));
			}
			else if (base.EntityManager.TryGetComponent<SpriteComponent>(val, ref val2))
			{
				_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((val.Value, val2)), !val2.Visible);
			}
		}
	}
}
