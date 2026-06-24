using System;
using Content.Shared.Drugs;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Client.Drugs;

public sealed class DrugOverlaySystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlayMan;

	[Dependency]
	private IRobustRandom _random;

	private RainbowOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectAppliedEvent>((EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectAppliedEvent>)OnApplied, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectRemovedEvent>((EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectRemovedEvent>)OnRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>((EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>((EntityEventRefHandler<SeeingRainbowsStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>)OnPlayerDetached, (Type[])null, (Type[])null);
		_overlay = new RainbowOverlay();
	}

	private void OnRemoved(Entity<SeeingRainbowsStatusEffectComponent> ent, ref StatusEffectRemovedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid target = args.Target;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != target))
		{
			_overlay.Intoxication = 0f;
			_overlay.TimeTicker = 0f;
			_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnApplied(Entity<SeeingRainbowsStatusEffectComponent> ent, ref StatusEffectAppliedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid target = args.Target;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != target))
		{
			_overlay.Phase = _random.NextFloat(MathF.PI * 2f);
			_overlayMan.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnPlayerAttached(Entity<SeeingRainbowsStatusEffectComponent> ent, ref StatusEffectRelayedEvent<LocalPlayerAttachedEvent> args)
	{
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnPlayerDetached(Entity<SeeingRainbowsStatusEffectComponent> ent, ref StatusEffectRelayedEvent<LocalPlayerDetachedEvent> args)
	{
		_overlay.Intoxication = 0f;
		_overlay.TimeTicker = 0f;
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
	}
}
