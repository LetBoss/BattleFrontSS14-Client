using System;
using Content.Client.Items;
using Content.Client.Radiation.UI;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Systems;
using Robust.Client.UserInterface;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;

namespace Content.Client.Radiation.Systems;

public sealed class GeigerSystem : SharedGeigerSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GeigerComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<GeigerComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).Subs.ItemStatus<GeigerComponent>((Func<Entity<GeigerComponent>, Control?>)((Entity<GeigerComponent> ent) => (Control?)(object)((!ent.Comp.ShowControl) ? null : new GeigerItemControl(Entity<GeigerComponent>.op_Implicit(ent)))));
	}

	private void OnHandleState(EntityUid uid, GeigerComponent component, ref AfterAutoHandleStateEvent args)
	{
		component.UiUpdateNeeded = true;
	}
}
