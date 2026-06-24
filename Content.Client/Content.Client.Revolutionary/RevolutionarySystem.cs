using System;
using Content.Shared.Revolutionary;
using Content.Shared.Revolutionary.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Revolutionary;

public sealed class RevolutionarySystem : SharedRevolutionarySystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RevolutionaryComponent, GetStatusIconsEvent>((EntityEventRefHandler<RevolutionaryComponent, GetStatusIconsEvent>)GetRevIcon, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadRevolutionaryComponent, GetStatusIconsEvent>((EntityEventRefHandler<HeadRevolutionaryComponent, GetStatusIconsEvent>)GetHeadRevIcon, (Type[])null, (Type[])null);
	}

	private void GetRevIcon(Entity<RevolutionaryComponent> ent, ref GetStatusIconsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		FactionIconPrototype item = default(FactionIconPrototype);
		if (!((EntitySystem)this).HasComp<HeadRevolutionaryComponent>(Entity<RevolutionaryComponent>.op_Implicit(ent)) && _prototype.TryIndex<FactionIconPrototype>(ent.Comp.StatusIcon, ref item))
		{
			args.StatusIcons.Add(item);
		}
	}

	private void GetHeadRevIcon(Entity<HeadRevolutionaryComponent> ent, ref GetStatusIconsEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		FactionIconPrototype item = default(FactionIconPrototype);
		if (_prototype.TryIndex<FactionIconPrototype>(ent.Comp.StatusIcon, ref item))
		{
			args.StatusIcons.Add(item);
		}
	}
}
