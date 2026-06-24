using System;
using Content.Shared.Beeper.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared.Beeper.Systems;

public sealed class BeeperSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ItemToggleSystem _toggle;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<BeeperComponent, ItemToggleComponent> query = ((EntitySystem)this).EntityQueryEnumerator<BeeperComponent, ItemToggleComponent>();
		EntityUid uid = default(EntityUid);
		BeeperComponent beeper = default(BeeperComponent);
		ItemToggleComponent toggle = default(ItemToggleComponent);
		while (query.MoveNext(ref uid, ref beeper, ref toggle))
		{
			if (toggle.Activated)
			{
				RunUpdate_Internal(uid, beeper);
			}
		}
	}

	public void SetIntervalScaling(EntityUid owner, BeeperComponent beeper, FixedPoint2 newScaling)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		newScaling = FixedPoint2.Clamp(newScaling, 0, 1);
		beeper.IntervalScaling = newScaling;
		RunUpdate_Internal(owner, beeper);
		((EntitySystem)this).Dirty(owner, (IComponent)(object)beeper, (MetaDataComponent)null);
	}

	public void SetInterval(EntityUid owner, BeeperComponent beeper, TimeSpan newInterval)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (newInterval < beeper.MinBeepInterval)
		{
			newInterval = beeper.MinBeepInterval;
		}
		if (newInterval > beeper.MaxBeepInterval)
		{
			newInterval = beeper.MaxBeepInterval;
		}
		beeper.Interval = newInterval;
		RunUpdate_Internal(owner, beeper);
		((EntitySystem)this).Dirty(owner, (IComponent)(object)beeper, (MetaDataComponent)null);
	}

	public void SetIntervalScaling(EntityUid owner, FixedPoint2 newScaling, BeeperComponent? beeper = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BeeperComponent>(owner, ref beeper, true))
		{
			SetIntervalScaling(owner, beeper, newScaling);
		}
	}

	public void SetMute(EntityUid owner, bool isMuted, BeeperComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BeeperComponent>(owner, ref comp, true))
		{
			comp.IsMuted = isMuted;
			((EntitySystem)this).Dirty(owner, (IComponent)(object)comp, (MetaDataComponent)null);
		}
	}

	private void UpdateBeepInterval(EntityUid owner, BeeperComponent beeper)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		float scalingFactor = beeper.IntervalScaling.Float();
		TimeSpan interval = (beeper.MaxBeepInterval - beeper.MinBeepInterval) * scalingFactor + beeper.MinBeepInterval;
		if (!(beeper.Interval == interval))
		{
			beeper.Interval = interval;
			((EntitySystem)this).Dirty(owner, (IComponent)(object)beeper, (MetaDataComponent)null);
		}
	}

	public void ForceUpdate(EntityUid owner, BeeperComponent? beeper = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<BeeperComponent>(owner, ref beeper, true))
		{
			RunUpdate_Internal(owner, beeper);
		}
	}

	private void RunUpdate_Internal(EntityUid owner, BeeperComponent beeper)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!_toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(owner)))
		{
			return;
		}
		UpdateBeepInterval(owner, beeper);
		if (!(beeper.NextBeep >= _timing.CurTime))
		{
			BeepPlayedEvent beepEvent = new BeepPlayedEvent(beeper.IsMuted);
			((EntitySystem)this).RaiseLocalEvent<BeepPlayedEvent>(owner, ref beepEvent, false);
			if (!beeper.IsMuted && _net.IsServer)
			{
				_audio.PlayPvs(beeper.BeepSound, owner, (AudioParams?)null);
			}
			beeper.LastBeepTime = _timing.CurTime;
		}
	}
}
