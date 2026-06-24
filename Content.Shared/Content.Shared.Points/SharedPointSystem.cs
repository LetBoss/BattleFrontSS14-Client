using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Shared.Points;

public abstract class SharedPointSystem : EntitySystem
{
	public void AdjustPointValue(NetUserId userId, FixedPoint2 value, EntityUid uid, PointManagerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PointManagerComponent>(uid, ref component, true))
		{
			if (!component.Points.TryGetValue(userId, out var current))
			{
				current = 0;
			}
			SetPointValue(userId, current + value, uid, component);
		}
	}

	public void SetPointValue(NetUserId userId, FixedPoint2 value, EntityUid uid, PointManagerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PointManagerComponent>(uid, ref component, true) && (!component.Points.TryGetValue(userId, out var current) || !(current == value)))
		{
			component.Points[userId] = value;
			component.Scoreboard = GetScoreboard(uid, component);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			PlayerPointChangedEvent ev = new PlayerPointChangedEvent(userId, value);
			((EntitySystem)this).RaiseLocalEvent<PlayerPointChangedEvent>(uid, ref ev, true);
		}
	}

	public FixedPoint2 GetPointValue(NetUserId userId, EntityUid uid, PointManagerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PointManagerComponent>(uid, ref component, true))
		{
			return FixedPoint2.Zero;
		}
		if (!component.Points.TryGetValue(userId, out var value))
		{
			return FixedPoint2.Zero;
		}
		return value;
	}

	public void EnsurePlayer(NetUserId userId, EntityUid uid, PointManagerComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PointManagerComponent>(uid, ref component, true) && !component.Points.ContainsKey(userId))
		{
			SetPointValue(userId, FixedPoint2.Zero, uid, component);
		}
	}

	public virtual FormattedMessage GetScoreboard(EntityUid uid, PointManagerComponent? component = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		return new FormattedMessage();
	}
}
