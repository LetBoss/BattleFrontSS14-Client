using System;
using Content.Client.Items.Systems;
using Content.Shared._PUBG.Armor;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._PUBG.Armor;

public sealed class PubgArmorVisualsSystem : EntitySystem
{
	[Dependency]
	private ItemSystem _item;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PubgArmorComponent, ComponentStartup>((EntityEventRefHandler<PubgArmorComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PubgArmorComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<PubgArmorComponent, AfterAutoHandleStateEvent>)OnAfterHandleState, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<PubgArmorComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_item.VisualsChanged(Entity<PubgArmorComponent>.op_Implicit(ent));
	}

	private void OnAfterHandleState(Entity<PubgArmorComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_item.VisualsChanged(Entity<PubgArmorComponent>.op_Implicit(ent));
	}
}
