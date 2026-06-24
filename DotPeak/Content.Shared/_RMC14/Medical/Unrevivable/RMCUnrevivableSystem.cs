// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Unrevivable.RMCUnrevivableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Content.Shared.Traits.Assorted;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Medical.Unrevivable;

public sealed class RMCUnrevivableSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCRevivableComponent, MobStateChangedEvent>(new EntityEventRefHandler<RMCRevivableComponent, MobStateChangedEvent>(this.OnMobstateChanged));
  }

  private void OnMobstateChanged(Entity<RMCRevivableComponent> ent, ref MobStateChangedEvent args)
  {
    ent.Comp.UnrevivableAt = args.NewMobState != MobState.Dead ? TimeSpan.Zero : this._timing.CurTime + ent.Comp.UnrevivableDelay;
    this.Dirty<RMCRevivableComponent>(ent);
  }

  public void AddRevivableTime(EntityUid uid, TimeSpan time)
  {
    RMCRevivableComponent comp;
    if (!this.TryComp<RMCRevivableComponent>(uid, out comp) || comp.UnrevivableAt == TimeSpan.Zero)
      return;
    comp.UnrevivableAt += time;
    this.Dirty(uid, (IComponent) comp);
  }

  public bool IsUnrevivable(EntityUid uid) => this.HasComp<UnrevivableComponent>(uid);

  public void MakeUnrevivable(Entity<RMCRevivableComponent?> ent, bool killLarva = true)
  {
    if (!this.Resolve<RMCRevivableComponent>(ent.Owner, ref ent.Comp, false))
      return;
    UnrevivableComponent unrevivableComponent = this.EnsureComp<UnrevivableComponent>((EntityUid) ent);
    unrevivableComponent.Analyzable = false;
    unrevivableComponent.Cloneable = false;
    unrevivableComponent.ReasonMessage = ent.Comp.UnrevivableReasonMessage;
    ent.Comp.KillLarva = killLarva;
    this.Dirty<RMCRevivableComponent>(ent);
  }

  public bool DoesKillLarvaOnUnrevivable(Entity<RMCRevivableComponent?> ent)
  {
    return this.Resolve<RMCRevivableComponent>(ent.Owner, ref ent.Comp, false) && ent.Comp.KillLarva;
  }

  public int GetUnrevivableStage(Entity<RMCRevivableComponent?> ent, int maxStages)
  {
    if (!this.Resolve<RMCRevivableComponent>(ent.Owner, ref ent.Comp, false) || ent.Comp.UnrevivableAt == TimeSpan.Zero)
      return 0;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan unrevivableAt = ent.Comp.UnrevivableAt;
    TimeSpan timeSpan1 = ent.Comp.UnrevivableAt - ent.Comp.UnrevivableDelay;
    TimeSpan timeSpan2 = timeSpan1;
    double num = (curTime - timeSpan2) / (unrevivableAt - timeSpan1);
    return (int) ((double) maxStages * num);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCRevivableComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCRevivableComponent>();
    EntityUid uid;
    RMCRevivableComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!this.IsUnrevivable(uid) && !(comp1.UnrevivableAt == TimeSpan.Zero) && !(this._timing.CurTime < comp1.UnrevivableAt))
        this.MakeUnrevivable((Entity<RMCRevivableComponent>) (uid, comp1));
    }
  }
}
