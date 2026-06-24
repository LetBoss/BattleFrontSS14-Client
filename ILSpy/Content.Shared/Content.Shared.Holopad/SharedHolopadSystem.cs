using System;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Holopad;

public abstract class SharedHolopadSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	public bool IsHolopadControlLocked(Entity<HolopadComponent> entity, EntityUid? user = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.ControlLockoutStartTime == TimeSpan.Zero)
		{
			return false;
		}
		if (entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds(entity.Comp.ControlLockoutDuration) < _timing.CurTime)
		{
			return false;
		}
		if (entity.Comp.ControlLockoutOwner.HasValue)
		{
			EntityUid? controlLockoutOwner = entity.Comp.ControlLockoutOwner;
			EntityUid? val = user;
			if (controlLockoutOwner.HasValue != val.HasValue || (controlLockoutOwner.HasValue && !(controlLockoutOwner.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				return true;
			}
		}
		return false;
	}

	public TimeSpan GetHolopadControlLockedPeriod(Entity<HolopadComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds(entity.Comp.ControlLockoutDuration) - _timing.CurTime;
	}

	public bool IsHolopadBroadcastOnCoolDown(Entity<HolopadComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (entity.Comp.ControlLockoutStartTime == TimeSpan.Zero)
		{
			return false;
		}
		if (entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds(entity.Comp.ControlLockoutCoolDown) < _timing.CurTime)
		{
			return false;
		}
		return true;
	}

	public TimeSpan GetHolopadBroadcastCoolDown(Entity<HolopadComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return entity.Comp.ControlLockoutStartTime + TimeSpan.FromSeconds(entity.Comp.ControlLockoutCoolDown) - _timing.CurTime;
	}
}
