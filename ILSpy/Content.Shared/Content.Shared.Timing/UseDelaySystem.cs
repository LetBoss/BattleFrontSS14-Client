using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Timing;

public sealed class UseDelaySystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private MetaDataSystem _metadata;

	private const string DefaultId = "default";

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<UseDelayComponent, MapInitEvent>((EntityEventRefHandler<UseDelayComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UseDelayComponent, EntityUnpausedEvent>((EntityEventRefHandler<UseDelayComponent, EntityUnpausedEvent>)OnUnpaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UseDelayComponent, ComponentGetState>((EntityEventRefHandler<UseDelayComponent, ComponentGetState>)OnDelayGetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UseDelayComponent, ComponentHandleState>((EntityEventRefHandler<UseDelayComponent, ComponentHandleState>)OnDelayHandleState, (Type[])null, (Type[])null);
	}

	private void OnDelayHandleState(Entity<UseDelayComponent> ent, ref ComponentHandleState args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ComponentHandleState)(ref args)).Current is UseDelayComponentState state))
		{
			return;
		}
		ent.Comp.Delays.Clear();
		foreach (var (key, delay) in state.Delays)
		{
			ent.Comp.Delays[key] = new UseDelayInfo(delay.Length, delay.StartTime, delay.EndTime);
		}
	}

	private void OnDelayGetState(Entity<UseDelayComponent> ent, ref ComponentGetState args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new UseDelayComponentState
		{
			Delays = ent.Comp.Delays
		};
	}

	private void OnMapInit(Entity<UseDelayComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		SetLength(Entity<UseDelayComponent>.op_Implicit((Entity<UseDelayComponent>.op_Implicit(ent), ent.Comp)), ent.Comp.Delay);
	}

	private void OnUnpaused(Entity<UseDelayComponent> ent, ref EntityUnpausedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		foreach (UseDelayInfo value in ent.Comp.Delays.Values)
		{
			value.EndTime += args.PausedTime;
		}
	}

	public bool SetLength(Entity<UseDelayComponent?> ent, TimeSpan length, string id = "default")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		UseDelayComponent comp = default(UseDelayComponent);
		((EntitySystem)this).EnsureComp<UseDelayComponent>(ent.Owner, ref comp);
		if (comp.Delays.TryGetValue(id, out UseDelayInfo entry))
		{
			if (entry.Length == length)
			{
				return true;
			}
			entry.Length = length;
		}
		else
		{
			comp.Delays.Add(id, new UseDelayInfo(length));
		}
		((EntitySystem)this).Dirty<UseDelayComponent>(ent, (MetaDataComponent)null);
		return true;
	}

	public bool IsDelayed(Entity<UseDelayComponent?> ent, string id = "default")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<UseDelayComponent>(ent.Owner, ref ent.Comp, false))
		{
			return false;
		}
		if (!ent.Comp.Delays.TryGetValue(id, out UseDelayInfo entry))
		{
			return false;
		}
		return entry.EndTime >= _gameTiming.CurTime;
	}

	public void CancelDelay(Entity<UseDelayComponent> ent, string id = "default")
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Delays.TryGetValue(id, out UseDelayInfo entry))
		{
			entry.EndTime = _gameTiming.CurTime;
			((EntitySystem)this).Dirty<UseDelayComponent>(ent, (MetaDataComponent)null);
		}
	}

	public bool TryGetDelayInfo(Entity<UseDelayComponent?> ent, [NotNullWhen(true)] out UseDelayInfo? info, string id = "default")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<UseDelayComponent>(ent.Owner, ref ent.Comp, false))
		{
			info = null;
			return false;
		}
		return ent.Comp.Delays.TryGetValue(id, out info);
	}

	public UseDelayInfo GetLastEndingDelay(Entity<UseDelayComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Delays.TryGetValue("default", out UseDelayInfo last))
		{
			return new UseDelayInfo(TimeSpan.Zero);
		}
		foreach (KeyValuePair<string, UseDelayInfo> entry in ent.Comp.Delays)
		{
			if (entry.Value.EndTime > last.EndTime)
			{
				last = entry.Value;
			}
		}
		return last;
	}

	public bool TryResetDelay(Entity<UseDelayComponent> ent, bool checkDelayed = false, string id = "default")
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (checkDelayed && IsDelayed(Entity<UseDelayComponent>.op_Implicit((ent.Owner, ent.Comp)), id))
		{
			return false;
		}
		if (!ent.Comp.Delays.TryGetValue(id, out UseDelayInfo entry))
		{
			return false;
		}
		TimeSpan curTime = (entry.StartTime = _gameTiming.CurTime);
		entry.EndTime = curTime - _metadata.GetPauseTime(Entity<UseDelayComponent>.op_Implicit(ent), (MetaDataComponent)null) + entry.Length;
		((EntitySystem)this).Dirty<UseDelayComponent>(ent, (MetaDataComponent)null);
		return true;
	}

	public bool TryResetDelay(EntityUid uid, bool checkDelayed = false, UseDelayComponent? component = null, string id = "default")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<UseDelayComponent>(uid, ref component, false))
		{
			return false;
		}
		return TryResetDelay(Entity<UseDelayComponent>.op_Implicit((uid, component)), checkDelayed, id);
	}

	public void ResetAllDelays(Entity<UseDelayComponent> ent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan curTime = _gameTiming.CurTime;
		foreach (UseDelayInfo entry in ent.Comp.Delays.Values)
		{
			entry.StartTime = curTime;
			entry.EndTime = curTime - _metadata.GetPauseTime(Entity<UseDelayComponent>.op_Implicit(ent), (MetaDataComponent)null) + entry.Length;
		}
		((EntitySystem)this).Dirty<UseDelayComponent>(ent, (MetaDataComponent)null);
	}
}
