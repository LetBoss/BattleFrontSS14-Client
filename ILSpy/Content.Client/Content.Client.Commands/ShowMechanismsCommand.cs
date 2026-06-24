using Content.Shared.Body.Organ;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

public sealed class ShowMechanismsCommand : LocalizedEntityCommands
{
	[Dependency]
	private SpriteSystem _spriteSystem;

	public override string Command => "showmechanisms";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<OrganComponent, SpriteComponent> val = base.EntityManager.AllEntityQueryEnumerator<OrganComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		OrganComponent organComponent = default(OrganComponent);
		SpriteComponent item2 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref organComponent, ref item2))
		{
			_spriteSystem.SetContainerOccluded(Entity<SpriteComponent>.op_Implicit((item, item2)), false);
		}
	}
}
