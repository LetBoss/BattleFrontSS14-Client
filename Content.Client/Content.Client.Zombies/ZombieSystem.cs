using System;
using System.Linq;
using Content.Shared.Humanoid;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Content.Shared.Zombies;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Zombies;

public sealed class ZombieSystem : SharedZombieSystem
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ZombieComponent, ComponentStartup>((ComponentEventHandler<ZombieComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ZombieComponent, GetStatusIconsEvent>((EntityEventRefHandler<ZombieComponent, GetStatusIconsEvent>)GetZombieIcon, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InitialInfectedComponent, GetStatusIconsEvent>((EntityEventRefHandler<InitialInfectedComponent, GetStatusIconsEvent>)GetInitialInfectedIcon, (Type[])null, (Type[])null);
	}

	private void GetZombieIcon(Entity<ZombieComponent> ent, ref GetStatusIconsEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		FactionIconPrototype item = _prototype.Index<FactionIconPrototype>(ent.Comp.StatusIcon);
		args.StatusIcons.Add(item);
	}

	private void GetInitialInfectedIcon(Entity<InitialInfectedComponent> ent, ref GetStatusIconsEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<ZombieComponent>(Entity<InitialInfectedComponent>.op_Implicit(ent)))
		{
			FactionIconPrototype item = _prototype.Index<FactionIconPrototype>(ent.Comp.StatusIcon);
			args.StatusIcons.Add(item);
		}
	}

	private void OnStartup(EntityUid uid, ZombieComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).HasComp<HumanoidAppearanceComponent>(uid) && ((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			for (int i = 0; i < val.AllLayers.Count(); i++)
			{
				_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, val)), i, component.SkinColor);
			}
		}
	}
}
