using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Client.DoAfter;

public sealed class DoAfterSystem : SharedDoAfterSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private MetaDataSystem _metadata;

	public override void Initialize()
	{
		base.Initialize();
		_overlay.AddOverlay((Overlay)(object)new DoAfterOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager, _prototype, GameTiming, _player));
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<DoAfterOverlay>();
	}

	public override void Update(float frameTime)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		ActiveDoAfterComponent active = default(ActiveDoAfterComponent);
		if (((EntitySystem)this).TryComp<ActiveDoAfterComponent>(localEntity, ref active) && !_metadata.EntityPaused(localEntity.Value, (MetaDataComponent)null))
		{
			TimeSpan curTime = GameTiming.CurTime;
			DoAfterComponent comp = ((EntitySystem)this).Comp<DoAfterComponent>(localEntity.Value);
			EntityQuery<TransformComponent> entityQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
			EntityQuery<HandsComponent> entityQuery2 = ((EntitySystem)this).GetEntityQuery<HandsComponent>();
			Update(localEntity.Value, active, comp, curTime, entityQuery, entityQuery2);
		}
	}

	public bool TryFindActiveDoAfter<T>(EntityUid entity, [NotNullWhen(true)] out Content.Shared.DoAfter.DoAfter? doAfter, [NotNullWhen(true)] out T? @event, out float progress) where T : DoAfterEvent
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		doAfter = null;
		@event = null;
		progress = 0f;
		ActiveDoAfterComponent activeDoAfterComponent = default(ActiveDoAfterComponent);
		if (!((EntitySystem)this).TryComp<ActiveDoAfterComponent>(localEntity, ref activeDoAfterComponent))
		{
			return false;
		}
		if (_metadata.EntityPaused(localEntity.Value, (MetaDataComponent)null))
		{
			return false;
		}
		DoAfterComponent doAfterComponent = ((EntitySystem)this).Comp<DoAfterComponent>(localEntity.Value);
		TimeSpan curTime = GameTiming.CurTime;
		foreach (Content.Shared.DoAfter.DoAfter value in doAfterComponent.DoAfters.Values)
		{
			if (!value.Cancelled)
			{
				EntityUid? target = value.Args.Target;
				if (target.HasValue && !(target.GetValueOrDefault() != entity) && value.Args.Event is T val)
				{
					@event = val;
					doAfter = value;
					progress = (float)Math.Min(1.0, (curTime - doAfter.StartTime).TotalSeconds / doAfter.Args.Delay.TotalSeconds);
					return true;
				}
			}
		}
		return false;
	}
}
