// Decompiled with JetBrains decompiler
// Type: Content.Shared.Points.SharedPointSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Shared.Points;

public abstract class SharedPointSystem : EntitySystem
{
  public void AdjustPointValue(
    NetUserId userId,
    FixedPoint2 value,
    EntityUid uid,
    PointManagerComponent? component = null)
  {
    if (!this.Resolve<PointManagerComponent>(uid, ref component))
      return;
    FixedPoint2 fixedPoint2;
    if (!component.Points.TryGetValue(userId, out fixedPoint2))
      fixedPoint2 = (FixedPoint2) 0;
    this.SetPointValue(userId, fixedPoint2 + value, uid, component);
  }

  public void SetPointValue(
    NetUserId userId,
    FixedPoint2 value,
    EntityUid uid,
    PointManagerComponent? component = null)
  {
    FixedPoint2 fixedPoint2;
    if (!this.Resolve<PointManagerComponent>(uid, ref component) || component.Points.TryGetValue(userId, out fixedPoint2) && fixedPoint2 == value)
      return;
    component.Points[userId] = value;
    component.Scoreboard = this.GetScoreboard(uid, component);
    this.Dirty(uid, (IComponent) component);
    PlayerPointChangedEvent args = new PlayerPointChangedEvent(userId, value);
    this.RaiseLocalEvent<PlayerPointChangedEvent>(uid, ref args, true);
  }

  public FixedPoint2 GetPointValue(
    NetUserId userId,
    EntityUid uid,
    PointManagerComponent? component = null)
  {
    FixedPoint2 fixedPoint2;
    return !this.Resolve<PointManagerComponent>(uid, ref component) || !component.Points.TryGetValue(userId, out fixedPoint2) ? FixedPoint2.Zero : fixedPoint2;
  }

  public void EnsurePlayer(NetUserId userId, EntityUid uid, PointManagerComponent? component = null)
  {
    if (!this.Resolve<PointManagerComponent>(uid, ref component) || component.Points.ContainsKey(userId))
      return;
    this.SetPointValue(userId, FixedPoint2.Zero, uid, component);
  }

  public virtual FormattedMessage GetScoreboard(EntityUid uid, PointManagerComponent? component = null)
  {
    return new FormattedMessage();
  }
}
