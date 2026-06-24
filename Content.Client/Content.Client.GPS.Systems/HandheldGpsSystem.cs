using System;
using Content.Client.GPS.UI;
using Content.Client.Items;
using Content.Shared.GPS.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.GPS.Systems;

public sealed class HandheldGpsSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).Subs.ItemStatus<HandheldGPSComponent>((Func<Entity<HandheldGPSComponent>, Control?>)((Entity<HandheldGPSComponent> ent) => (Control?)(object)new HandheldGpsStatusControl(ent)));
	}
}
