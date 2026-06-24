using System;
using Content.Shared.Body.Components;
using Content.Shared.Morgue.Components;
using Content.Shared.Standing;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Morgue;

public sealed class EntityStorageLayingDownOverrideSystem : EntitySystem
{
	[Dependency]
	private StandingStateSystem _standing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageLayingDownOverrideComponent, StorageBeforeCloseEvent>((ComponentEventRefHandler<EntityStorageLayingDownOverrideComponent, StorageBeforeCloseEvent>)OnBeforeClose, (Type[])null, (Type[])null);
	}

	private void OnBeforeClose(EntityUid uid, EntityStorageLayingDownOverrideComponent component, ref StorageBeforeCloseEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!component.Enabled)
		{
			return;
		}
		foreach (EntityUid ent in args.Contents)
		{
			if (((EntitySystem)this).HasComp<BodyComponent>(ent) && !_standing.IsDown(ent))
			{
				args.Contents.Remove(ent);
			}
		}
	}
}
