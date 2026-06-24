using System;
using Content.Shared.Audio;
using Content.Shared.Destructible;
using Content.Shared.Examine;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Delivery;

public sealed class DeliveryModifierSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private NameModifierSystem _nameModifier;

	[Dependency]
	private SharedDeliverySystem _delivery;

	[Dependency]
	private SharedExplosionSystem _explosion;

	[Dependency]
	private SharedAmbientSoundSystem _ambientSound;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DeliveryRandomMultiplierComponent, MapInitEvent>((EntityEventRefHandler<DeliveryRandomMultiplierComponent, MapInitEvent>)OnRandomMultiplierMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryRandomMultiplierComponent, GetDeliveryMultiplierEvent>((EntityEventRefHandler<DeliveryRandomMultiplierComponent, GetDeliveryMultiplierEvent>)OnGetRandomMultiplier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryPriorityComponent, MapInitEvent>((EntityEventRefHandler<DeliveryPriorityComponent, MapInitEvent>)OnPriorityMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryPriorityComponent, DeliveryUnlockedEvent>((EntityEventRefHandler<DeliveryPriorityComponent, DeliveryUnlockedEvent>)OnPriorityDelivered, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryPriorityComponent, ExaminedEvent>((EntityEventRefHandler<DeliveryPriorityComponent, ExaminedEvent>)OnPriorityExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryPriorityComponent, GetDeliveryMultiplierEvent>((EntityEventRefHandler<DeliveryPriorityComponent, GetDeliveryMultiplierEvent>)OnGetPriorityMultiplier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryFragileComponent, MapInitEvent>((EntityEventRefHandler<DeliveryFragileComponent, MapInitEvent>)OnFragileMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryFragileComponent, BreakageEventArgs>((EntityEventRefHandler<DeliveryFragileComponent, BreakageEventArgs>)OnFragileBreakage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryFragileComponent, ExaminedEvent>((EntityEventRefHandler<DeliveryFragileComponent, ExaminedEvent>)OnFragileExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryFragileComponent, GetDeliveryMultiplierEvent>((EntityEventRefHandler<DeliveryFragileComponent, GetDeliveryMultiplierEvent>)OnGetFragileMultiplier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryBombComponent, ComponentStartup>((EntityEventRefHandler<DeliveryBombComponent, ComponentStartup>)OnExplosiveStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PrimedDeliveryBombComponent, MapInitEvent>((EntityEventRefHandler<PrimedDeliveryBombComponent, MapInitEvent>)OnPrimedExplosiveMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryBombComponent, ExaminedEvent>((EntityEventRefHandler<DeliveryBombComponent, ExaminedEvent>)OnExplosiveExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryBombComponent, GetDeliveryMultiplierEvent>((EntityEventRefHandler<DeliveryBombComponent, GetDeliveryMultiplierEvent>)OnGetExplosiveMultiplier, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryBombComponent, DeliveryUnlockedEvent>((EntityEventRefHandler<DeliveryBombComponent, DeliveryUnlockedEvent>)OnExplosiveUnlock, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryBombComponent, DeliveryPriorityExpiredEvent>((EntityEventRefHandler<DeliveryBombComponent, DeliveryPriorityExpiredEvent>)OnExplosiveExpire, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DeliveryBombComponent, BreakageEventArgs>((EntityEventRefHandler<DeliveryBombComponent, BreakageEventArgs>)OnExplosiveBreak, (Type[])null, (Type[])null);
	}

	private void OnRandomMultiplierMapInit(Entity<DeliveryRandomMultiplierComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.CurrentMultiplierOffset = _random.NextFloat(ent.Comp.MinMultiplierOffset, ent.Comp.MaxMultiplierOffset);
		((EntitySystem)this).Dirty<DeliveryRandomMultiplierComponent>(ent, (MetaDataComponent)null);
	}

	private void OnGetRandomMultiplier(Entity<DeliveryRandomMultiplierComponent> ent, ref GetDeliveryMultiplierEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.AdditiveMultiplier += ent.Comp.CurrentMultiplierOffset;
	}

	private void OnPriorityMapInit(Entity<DeliveryPriorityComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.DeliverUntilTime = _timing.CurTime + ent.Comp.DeliveryTime;
		_delivery.UpdatePriorityVisuals(ent);
		((EntitySystem)this).Dirty<DeliveryPriorityComponent>(ent, (MetaDataComponent)null);
	}

	private void OnPriorityDelivered(Entity<DeliveryPriorityComponent> ent, ref DeliveryUnlockedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Expired)
		{
			ent.Comp.Delivered = true;
			((EntitySystem)this).Dirty<DeliveryPriorityComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnPriorityExamine(Entity<DeliveryPriorityComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		string trueName = _nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		TimeSpan timeLeft = ent.Comp.DeliverUntilTime - _timing.CurTime;
		if (ent.Comp.Delivered)
		{
			args.PushMarkup(base.Loc.GetString("delivery-priority-delivered-examine", (ValueTuple<string, object>)("type", trueName)));
		}
		else if (_timing.CurTime < ent.Comp.DeliverUntilTime)
		{
			args.PushMarkup(base.Loc.GetString("delivery-priority-examine", (ValueTuple<string, object>)("type", trueName), (ValueTuple<string, object>)("time", timeLeft.ToString("mm\\:ss"))));
		}
		else
		{
			args.PushMarkup(base.Loc.GetString("delivery-priority-expired-examine", (ValueTuple<string, object>)("type", trueName)));
		}
	}

	private void OnGetPriorityMultiplier(Entity<DeliveryPriorityComponent> ent, ref GetDeliveryMultiplierEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.CurTime < ent.Comp.DeliverUntilTime)
		{
			args.AdditiveMultiplier += ent.Comp.InTimeMultiplierOffset;
		}
		else
		{
			args.AdditiveMultiplier += ent.Comp.ExpiredMultiplierOffset;
		}
	}

	private void OnFragileMapInit(Entity<DeliveryFragileComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_delivery.UpdateBrokenVisuals(ent, isFragile: true);
	}

	private void OnFragileBreakage(Entity<DeliveryFragileComponent> ent, ref BreakageEventArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Broken = true;
		_delivery.UpdateBrokenVisuals(ent, isFragile: true);
		((EntitySystem)this).Dirty<DeliveryFragileComponent>(ent, (MetaDataComponent)null);
	}

	private void OnFragileExamine(Entity<DeliveryFragileComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		string trueName = _nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		if (ent.Comp.Broken)
		{
			args.PushMarkup(base.Loc.GetString("delivery-fragile-broken-examine", (ValueTuple<string, object>)("type", trueName)));
		}
		else
		{
			args.PushMarkup(base.Loc.GetString("delivery-fragile-examine", (ValueTuple<string, object>)("type", trueName)));
		}
	}

	private void OnGetFragileMultiplier(Entity<DeliveryFragileComponent> ent, ref GetDeliveryMultiplierEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Broken)
		{
			args.AdditiveMultiplier += ent.Comp.BrokenMultiplierOffset;
		}
		else
		{
			args.AdditiveMultiplier += ent.Comp.IntactMultiplierOffset;
		}
	}

	private void OnExplosiveStartup(Entity<DeliveryBombComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_delivery.UpdateBombVisuals(ent);
	}

	private void OnPrimedExplosiveMapInit(Entity<PrimedDeliveryBombComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DeliveryBombComponent bomb = default(DeliveryBombComponent);
		if (((EntitySystem)this).TryComp<DeliveryBombComponent>(Entity<PrimedDeliveryBombComponent>.op_Implicit(ent), ref bomb))
		{
			bomb.NextExplosionRetry = _timing.CurTime;
		}
	}

	private void OnExplosiveExamine(Entity<DeliveryBombComponent> ent, ref ExaminedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		string trueName = _nameModifier.GetBaseName(Entity<NameModifierComponent>.op_Implicit(ent.Owner));
		if (((EntitySystem)this).HasComp<PrimedDeliveryBombComponent>(Entity<DeliveryBombComponent>.op_Implicit(ent)))
		{
			args.PushMarkup(base.Loc.GetString("delivery-bomb-primed-examine", (ValueTuple<string, object>)("type", trueName)));
		}
		else
		{
			args.PushMarkup(base.Loc.GetString("delivery-bomb-examine", (ValueTuple<string, object>)("type", trueName)));
		}
	}

	private void OnGetExplosiveMultiplier(Entity<DeliveryBombComponent> ent, ref GetDeliveryMultiplierEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.MultiplicativeMultiplier += ent.Comp.SpesoMultiplier;
	}

	private void OnExplosiveUnlock(Entity<DeliveryBombComponent> ent, ref DeliveryUnlockedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.PrimeOnUnlock)
		{
			PrimeBombDelivery(ent);
		}
	}

	private void OnExplosiveExpire(Entity<DeliveryBombComponent> ent, ref DeliveryPriorityExpiredEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.PrimeOnExpire)
		{
			PrimeBombDelivery(ent);
		}
	}

	private void OnExplosiveBreak(Entity<DeliveryBombComponent> ent, ref BreakageEventArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.PrimeOnBreakage)
		{
			PrimeBombDelivery(ent);
		}
	}

	public void PrimeBombDelivery(Entity<DeliveryBombComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<PrimedDeliveryBombComponent>(Entity<DeliveryBombComponent>.op_Implicit(ent));
		_delivery.UpdateBombVisuals(ent);
		_ambientSound.SetAmbience(Entity<DeliveryBombComponent>.op_Implicit(ent), value: true);
	}

	public override void Update(float frameTime)
	{
		((EntitySystem)this).Update(frameTime);
		UpdatePriorty(frameTime);
		UpdateBomb(frameTime);
	}

	private void UpdatePriorty(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<DeliveryPriorityComponent> priorityQuery = ((EntitySystem)this).EntityQueryEnumerator<DeliveryPriorityComponent>();
		TimeSpan curTime = _timing.CurTime;
		EntityUid uid = default(EntityUid);
		DeliveryPriorityComponent priorityData = default(DeliveryPriorityComponent);
		while (priorityQuery.MoveNext(ref uid, ref priorityData))
		{
			if (!priorityData.Expired && !priorityData.Delivered && priorityData.DeliverUntilTime < curTime)
			{
				priorityData.Expired = true;
				_delivery.UpdatePriorityVisuals(Entity<DeliveryPriorityComponent>.op_Implicit((uid, priorityData)));
				((EntitySystem)this).Dirty(uid, (IComponent)(object)priorityData, (MetaDataComponent)null);
				((EntitySystem)this).RaiseLocalEvent<DeliveryPriorityExpiredEvent>(uid, default(DeliveryPriorityExpiredEvent), false);
			}
		}
	}

	private void UpdateBomb(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<PrimedDeliveryBombComponent, DeliveryBombComponent> bombQuery = ((EntitySystem)this).EntityQueryEnumerator<PrimedDeliveryBombComponent, DeliveryBombComponent>();
		TimeSpan curTime = _timing.CurTime;
		EntityUid uid = default(EntityUid);
		PrimedDeliveryBombComponent primedDeliveryBombComponent = default(PrimedDeliveryBombComponent);
		DeliveryBombComponent bombData = default(DeliveryBombComponent);
		while (bombQuery.MoveNext(ref uid, ref primedDeliveryBombComponent, ref bombData))
		{
			if (!(bombData.NextExplosionRetry > curTime))
			{
				bombData.NextExplosionRetry += bombData.ExplosionRetryDelay;
				if (_net.IsServer && _random.NextFloat() < bombData.ExplosionChance)
				{
					_explosion.TriggerExplosive(uid);
				}
				bombData.ExplosionChance += bombData.ExplosionChanceRetryIncrease;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)bombData, (MetaDataComponent)null);
			}
		}
	}
}
