using System;
using Content.Shared.Body.Organ;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands;

public sealed class HideMechanismsCommand : LocalizedEntityCommands
{
	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SpriteSystem _spriteSystem;

	public override string Command => "hidemechanisms";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<OrganComponent, SpriteComponent> val = base.EntityManager.AllEntityQueryEnumerator<OrganComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		OrganComponent organComponent = default(OrganComponent);
		SpriteComponent item = default(SpriteComponent);
		BaseContainer val3 = default(BaseContainer);
		while (val.MoveNext(ref val2, ref organComponent, ref item))
		{
			_spriteSystem.SetContainerOccluded(Entity<SpriteComponent>.op_Implicit((val2, item)), false);
			EntityUid item2 = val2;
			while (_containerSystem.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent, MetaDataComponent>)(item2, null, null)), ref val3))
			{
				if (!val3.ShowContents)
				{
					_spriteSystem.SetContainerOccluded(Entity<SpriteComponent>.op_Implicit((val2, item)), true);
					break;
				}
				item2 = val3.Owner;
			}
		}
	}
}
