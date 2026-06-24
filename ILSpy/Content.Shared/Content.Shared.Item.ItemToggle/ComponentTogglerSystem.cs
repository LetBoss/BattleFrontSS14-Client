using System;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Item.ItemToggle;

public sealed class ComponentTogglerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ComponentTogglerComponent, ItemToggledEvent>((EntityEventRefHandler<ComponentTogglerComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnToggled(Entity<ComponentTogglerComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (args.Activated)
		{
			EntityUid target = (ent.Comp.Parent ? ((EntitySystem)this).Transform(Entity<ComponentTogglerComponent>.op_Implicit(ent)).ParentUid : ent.Owner);
			if (!((EntitySystem)this).TerminatingOrDeleted(target, (MetaDataComponent)null))
			{
				ent.Comp.Target = target;
				base.EntityManager.AddComponents(target, ent.Comp.Components, true);
			}
		}
		else if (ent.Comp.Target.HasValue && !((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Target.Value, (MetaDataComponent)null))
		{
			base.EntityManager.RemoveComponents(ent.Comp.Target.Value, ent.Comp.RemoveComponents ?? ent.Comp.Components);
		}
	}
}
