using System;
using Content.Shared.Drowsiness;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.Drowsiness;

public sealed class DrowsinessSystem : SharedDrowsinessSystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IOverlayManager _overlayMan;

	[Dependency]
	private SharedStatusEffectsSystem _statusEffects;

	private DrowsinessOverlay _overlay;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectAppliedEvent>((EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectAppliedEvent>)OnDrowsinessApply, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectRemovedEvent>((EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectRemovedEvent>)OnDrowsinessShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>((EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerAttachedEvent>>)OnStatusEffectPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>((EntityEventRefHandler<DrowsinessStatusEffectComponent, StatusEffectRelayedEvent<LocalPlayerDetachedEvent>>)OnStatusEffectPlayerDetached, (Type[])null, (Type[])null);
		_overlay = new DrowsinessOverlay();
	}

	private void OnDrowsinessApply(Entity<DrowsinessStatusEffectComponent> ent, ref StatusEffectAppliedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid target = args.Target;
		if (localEntity.HasValue && localEntity.GetValueOrDefault() == target)
		{
			_overlayMan.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnDrowsinessShutdown(Entity<DrowsinessStatusEffectComponent> ent, ref StatusEffectRemovedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid target = args.Target;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != target) && !_statusEffects.HasEffectComp<DrowsinessStatusEffectComponent>((EntityUid?)((ISharedPlayerManager)_player).LocalEntity.Value))
		{
			_overlay.CurrentPower = 0f;
			_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		}
	}

	private void OnStatusEffectPlayerAttached(Entity<DrowsinessStatusEffectComponent> ent, ref StatusEffectRelayedEvent<LocalPlayerAttachedEvent> args)
	{
		_overlayMan.AddOverlay((Overlay)(object)_overlay);
	}

	private void OnStatusEffectPlayerDetached(Entity<DrowsinessStatusEffectComponent> ent, ref StatusEffectRelayedEvent<LocalPlayerDetachedEvent> args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((ISharedPlayerManager)_player).LocalEntity.HasValue && !_statusEffects.HasEffectComp<DrowsinessStatusEffectComponent>((EntityUid?)((ISharedPlayerManager)_player).LocalEntity.Value))
		{
			_overlay.CurrentPower = 0f;
			_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
		}
	}
}
