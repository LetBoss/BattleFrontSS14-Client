using System;
using Content.Client.Effects;
using Content.Client.Smoking;
using Content.Shared.Chemistry.Components;
using Content.Shared.Polymorph.Components;
using Content.Shared.Polymorph.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Polymorph.Systems;

public sealed class ChameleonProjectorSystem : SharedChameleonProjectorSystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	private EntityQuery<AppearanceComponent> _appearanceQuery;

	private EntityQuery<SpriteComponent> _spriteQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_appearanceQuery = ((EntitySystem)this).GetEntityQuery<AppearanceComponent>();
		_spriteQuery = ((EntitySystem)this).GetEntityQuery<SpriteComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguiseComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ChameleonDisguiseComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguisedComponent, ComponentStartup>((EntityEventRefHandler<ChameleonDisguisedComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguisedComponent, ComponentShutdown>((EntityEventRefHandler<ChameleonDisguisedComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChameleonDisguisedComponent, GetFlashEffectTargetEvent>((EntityEventRefHandler<ChameleonDisguisedComponent, GetFlashEffectTargetEvent>)OnGetFlashEffectTargetEvent, (Type[])null, (Type[])null);
	}

	private void OnHandleState(Entity<ChameleonDisguiseComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		CopyComp<SpriteComponent>(ent);
		CopyComp<GenericVisualizerComponent>(ent);
		CopyComp<SolutionContainerVisualsComponent>(ent);
		CopyComp<BurnStateVisualsComponent>(ent);
		AppearanceComponent val = default(AppearanceComponent);
		if (_appearanceQuery.TryComp(Entity<ChameleonDisguiseComponent>.op_Implicit(ent), ref val))
		{
			_appearance.QueueUpdate(Entity<ChameleonDisguiseComponent>.op_Implicit(ent), val);
		}
	}

	private void OnStartup(Entity<ChameleonDisguisedComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (_spriteQuery.TryComp(Entity<ChameleonDisguisedComponent>.op_Implicit(ent), ref val))
		{
			ent.Comp.WasVisible = val.Visible;
			_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), false);
		}
	}

	private void OnShutdown(Entity<ChameleonDisguisedComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (_spriteQuery.TryComp(Entity<ChameleonDisguisedComponent>.op_Implicit(ent), ref item))
		{
			_sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, item)), ent.Comp.WasVisible);
		}
	}

	private void OnGetFlashEffectTargetEvent(Entity<ChameleonDisguisedComponent> ent, ref GetFlashEffectTargetEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Target = ent.Comp.Disguise;
	}
}
