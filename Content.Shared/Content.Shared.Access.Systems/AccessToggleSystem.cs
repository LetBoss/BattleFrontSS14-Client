using System;
using Content.Shared.Access.Components;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Access.Systems;

public sealed class AccessToggleSystem : EntitySystem
{
	[Dependency]
	private SharedAccessSystem _access;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AccessToggleComponent, ItemToggledEvent>((EntityEventRefHandler<AccessToggleComponent, ItemToggledEvent>)OnToggled, (Type[])null, (Type[])null);
	}

	private void OnToggled(Entity<AccessToggleComponent> ent, ref ItemToggledEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_access.SetAccessEnabled(Entity<AccessToggleComponent>.op_Implicit(ent), args.Activated);
	}
}
