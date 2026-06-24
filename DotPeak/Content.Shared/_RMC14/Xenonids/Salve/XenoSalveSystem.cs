// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Salve.XenoSalveSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Heal;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Salve;

public sealed class XenoSalveSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RecentlySalvedComponent, ComponentStartup>(new EntityEventRefHandler<RecentlySalvedComponent, ComponentStartup>(this.OnSalveAdded));
    this.SubscribeLocalEvent<RecentlySalvedComponent, ComponentShutdown>(new EntityEventRefHandler<RecentlySalvedComponent, ComponentShutdown>(this.OnSalveRemoved));
  }

  private void OnSalveAdded(Entity<RecentlySalvedComponent> xeno, ref ComponentStartup args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoHealerVisuals.Gooped, (object) true);
  }

  private void OnSalveRemoved(Entity<RecentlySalvedComponent> xeno, ref ComponentShutdown args)
  {
    if (this._timing.ApplyingState)
      return;
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoHealerVisuals.Gooped, (object) false);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RecentlySalvedComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RecentlySalvedComponent>();
    EntityUid uid;
    RecentlySalvedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.ExpiresAt))
        this.RemCompDeferred<RecentlySalvedComponent>(uid);
    }
  }
}
