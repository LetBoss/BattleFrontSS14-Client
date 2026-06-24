using System;
using Content.Shared.Alert;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Gravity;

public abstract class SharedGravitySystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	private sealed class GravityComponentState : ComponentState
	{
		public bool Enabled { get; }

		public GravityComponentState(bool enabled)
		{
			Enabled = enabled;
		}
	}

	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private AlertsSystem _alerts;

	public static readonly ProtoId<AlertPrototype> WeightlessAlert = ProtoId<AlertPrototype>.op_Implicit("Weightless");

	private EntityQuery<GravityComponent> _gravityQuery;

	protected const float GravityKick = 100f;

	protected const float ShakeCooldown = 0.2f;

	public bool IsWeightless(EntityUid uid, PhysicsComponent? body = null, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Resolve<PhysicsComponent>(uid, ref body, false);
		if (body == null || (body.BodyType & 4) > 0)
		{
			return false;
		}
		MovementIgnoreGravityComponent ignoreGravityComponent = default(MovementIgnoreGravityComponent);
		if (((EntitySystem)this).TryComp<MovementIgnoreGravityComponent>(uid, ref ignoreGravityComponent))
		{
			return ignoreGravityComponent.Weightless;
		}
		IsWeightlessEvent ev = new IsWeightlessEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<IsWeightlessEvent>(uid, ref ev, false);
		if (ev.Handled)
		{
			return ev.IsWeightless;
		}
		if (!((EntitySystem)this).Resolve(uid, ref xform, true))
		{
			return true;
		}
		if (EntityGridOrMapHaveGravity(Entity<TransformComponent>.op_Implicit((uid, xform))))
		{
			return false;
		}
		return true;
	}

	public bool EntityOnGravitySupportingGridOrMap(Entity<TransformComponent?> entity)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ref TransformComponent comp = ref entity.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).Transform(Entity<TransformComponent>.op_Implicit(entity));
		}
		if (!_gravityQuery.HasComp(entity.Comp.GridUid))
		{
			return _gravityQuery.HasComp(entity.Comp.MapUid);
		}
		return true;
	}

	public bool EntityGridOrMapHaveGravity(Entity<TransformComponent?> entity)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		ref TransformComponent comp = ref entity.Comp;
		if (comp == null)
		{
			comp = ((EntitySystem)this).Transform(Entity<TransformComponent>.op_Implicit(entity));
		}
		GravityComponent gravity = default(GravityComponent);
		if (!_gravityQuery.TryComp(entity.Comp.GridUid, ref gravity) || !gravity.Enabled)
		{
			GravityComponent mapGravity = default(GravityComponent);
			if (_gravityQuery.TryComp(entity.Comp.MapUid, ref mapGravity))
			{
				return mapGravity.Enabled;
			}
			return false;
		}
		return true;
	}

	public override void Initialize()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GridInitializeEvent>((EntityEventHandler<GridInitializeEvent>)OnGridInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AlertSyncEvent>((EntityEventHandler<AlertSyncEvent>)OnAlertsSync, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AlertsComponent, EntParentChangedMessage>((ComponentEventRefHandler<AlertsComponent, EntParentChangedMessage>)OnAlertsParentChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GravityChangedEvent>((EntityEventRefHandler<GravityChangedEvent>)OnGravityChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GravityComponent, ComponentGetState>((ComponentEventRefHandler<GravityComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GravityComponent, ComponentHandleState>((ComponentEventRefHandler<GravityComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		_gravityQuery = ((EntitySystem)this).GetEntityQuery<GravityComponent>();
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		UpdateShake();
	}

	private void OnHandleState(EntityUid uid, GravityComponent component, ref ComponentHandleState args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is GravityComponentState state && component.EnabledVV != state.Enabled)
		{
			component.EnabledVV = state.Enabled;
			GravityChangedEvent ev = new GravityChangedEvent(uid, component.EnabledVV);
			((EntitySystem)this).RaiseLocalEvent<GravityChangedEvent>(uid, ref ev, true);
		}
	}

	private void OnGetState(EntityUid uid, GravityComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new GravityComponentState(component.EnabledVV);
	}

	private void OnGravityChange(ref GravityChangedEvent ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<AlertsComponent, TransformComponent> alerts = ((EntitySystem)this).AllEntityQuery<AlertsComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		AlertsComponent alertsComponent = default(AlertsComponent);
		TransformComponent xform = default(TransformComponent);
		while (alerts.MoveNext(ref uid, ref alertsComponent, ref xform))
		{
			EntityUid? gridUid = xform.GridUid;
			EntityUid changedGridIndex = ev.ChangedGridIndex;
			if (gridUid.HasValue && !(gridUid.GetValueOrDefault() != changedGridIndex))
			{
				if (!ev.HasGravity)
				{
					_alerts.ShowAlert(uid, WeightlessAlert);
				}
				else
				{
					_alerts.ClearAlert(uid, WeightlessAlert);
				}
			}
		}
	}

	private void OnAlertsSync(AlertSyncEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (IsWeightless(ev.Euid))
		{
			_alerts.ShowAlert(ev.Euid, WeightlessAlert);
		}
		else
		{
			_alerts.ClearAlert(ev.Euid, WeightlessAlert);
		}
	}

	private void OnAlertsParentChange(EntityUid uid, AlertsComponent component, ref EntParentChangedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (IsWeightless(uid))
		{
			_alerts.ShowAlert(uid, WeightlessAlert);
		}
		else
		{
			_alerts.ClearAlert(uid, WeightlessAlert);
		}
	}

	private void OnGridInit(GridInitializeEvent ev)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<GravityComponent>(ev.EntityUid);
	}

	private void UpdateShake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan curTime = Timing.CurTime;
		EntityQuery<GravityComponent> gravityQuery = ((EntitySystem)this).GetEntityQuery<GravityComponent>();
		EntityQueryEnumerator<GravityShakeComponent> query = ((EntitySystem)this).EntityQueryEnumerator<GravityShakeComponent>();
		EntityUid uid = default(EntityUid);
		GravityShakeComponent comp = default(GravityShakeComponent);
		GravityComponent gravity = default(GravityComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (comp.NextShake <= curTime)
			{
				if (comp.ShakeTimes == 0 || !gravityQuery.TryGetComponent(uid, ref gravity))
				{
					((EntitySystem)this).RemCompDeferred<GravityShakeComponent>(uid);
					continue;
				}
				ShakeGrid(uid, gravity);
				comp.ShakeTimes--;
				comp.NextShake += TimeSpan.FromSeconds(0.20000000298023224);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}

	public void StartGridShake(EntityUid uid, GravityComponent? gravity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Terminating(uid, (MetaDataComponent)null) && ((EntitySystem)this).Resolve<GravityComponent>(uid, ref gravity, false))
		{
			GravityShakeComponent shake = default(GravityShakeComponent);
			if (!((EntitySystem)this).TryComp<GravityShakeComponent>(uid, ref shake))
			{
				shake = ((EntitySystem)this).AddComp<GravityShakeComponent>(uid);
				shake.NextShake = Timing.CurTime;
			}
			shake.ShakeTimes = 10;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)shake, (MetaDataComponent)null);
		}
	}

	protected virtual void ShakeGrid(EntityUid uid, GravityComponent? comp = null)
	{
	}
}
