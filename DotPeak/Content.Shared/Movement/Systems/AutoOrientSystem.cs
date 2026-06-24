// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.AutoOrientSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class AutoOrientSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _cfgManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedMoverController _mover;
  private TimeSpan _delay = TimeSpan.Zero;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AutoOrientComponent, EntParentChangedMessage>(new EntityEventRefHandler<AutoOrientComponent, EntParentChangedMessage>(this.OnEntParentChanged));
    this.Subs.CVar<double>(this._cfgManager, CCVars.AutoOrientDelay, new Action<double>(this.OnAutoOrient), true);
  }

  private void OnAutoOrient(double obj) => this._delay = TimeSpan.FromSeconds(obj);

  private void OnEntParentChanged(Entity<AutoOrientComponent> ent, ref EntParentChangedMessage args)
  {
    ent.Comp.NextChange = new TimeSpan?(this._timing.CurTime + this._delay);
    this.Dirty<AutoOrientComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<AutoOrientComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AutoOrientComponent>();
    EntityUid uid;
    AutoOrientComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      TimeSpan? nextChange = comp1.NextChange;
      TimeSpan curTime = this._timing.CurTime;
      if ((nextChange.HasValue ? (nextChange.GetValueOrDefault() <= curTime ? 1 : 0) : 0) != 0)
      {
        comp1.NextChange = new TimeSpan?();
        this.Dirty(uid, (IComponent) comp1);
        this._mover.ResetCamera(uid);
      }
    }
  }
}
