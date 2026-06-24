// Decompiled with JetBrains decompiler
// Type: Content.Shared.Xenoarchaeology.Artifact.XAT.XATTimerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Shared.Xenoarchaeology.Artifact.XAT;

public sealed class XATTimerSystem : BaseQueryUpdateXATSystem<XATTimerComponent>
{
  [Dependency]
  private IRobustRandom _robustRandom;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XATTimerComponent, MapInitEvent>(new EntityEventRefHandler<XATTimerComponent, MapInitEvent>(this.OnMapInit));
    this.XATSubscribeDirectEvent<ExaminedEvent>(new BaseXATSystem<XATTimerComponent>.XATEventHandler<ExaminedEvent>(this.OnExamine));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<XATTimerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XATTimerComponent>();
    EntityUid uid;
    XATTimerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(this.Timing.CurTime < comp1.NextActivation))
      {
        comp1.NextActivation += this.GetNextDelay(comp1);
        this.Dirty(uid, (IComponent) comp1);
      }
    }
  }

  protected override void UpdateXAT(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATTimerComponent, XenoArtifactNodeComponent> node,
    float frameTime)
  {
    if (!(this.Timing.CurTime > node.Comp1.NextActivation))
      return;
    this.Trigger(artifact, node);
  }

  private void OnMapInit(Entity<XATTimerComponent> ent, ref MapInitEvent args)
  {
    TimeSpan nextDelay = this.GetNextDelay((XATTimerComponent) ent);
    ent.Comp.NextActivation = this.Timing.CurTime + nextDelay;
    this.Dirty<XATTimerComponent>(ent);
  }

  private void OnExamine(
    Entity<XenoArtifactComponent> artifact,
    Entity<XATTimerComponent, XenoArtifactNodeComponent> node,
    ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("xenoarch-trigger-examine-timer", ("time", (object) MathF.Ceiling((float) (node.Comp1.NextActivation - this.Timing.CurTime).TotalSeconds))));
  }

  private TimeSpan GetNextDelay(XATTimerComponent comp)
  {
    return TimeSpan.FromSeconds((long) comp.PossibleDelayInSeconds.Next(this._robustRandom));
  }
}
