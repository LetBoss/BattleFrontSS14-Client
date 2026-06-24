// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Generator.ActiveGeneratorRevvingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Power.Generator;

public sealed class ActiveGeneratorRevvingSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActiveGeneratorRevvingComponent, AnchorStateChangedEvent>(new ComponentEventHandler<ActiveGeneratorRevvingComponent, AnchorStateChangedEvent>(this.OnAnchorStateChanged));
  }

  private void OnAnchorStateChanged(
    EntityUid uid,
    ActiveGeneratorRevvingComponent component,
    AnchorStateChangedEvent args)
  {
    if (args.Anchored)
      return;
    this.StopAutoRevving(uid);
  }

  public void StartAutoRevving(EntityUid uid, ActiveGeneratorRevvingComponent? component = null)
  {
    if (this.Resolve<ActiveGeneratorRevvingComponent>(uid, ref component, false))
      component.CurrentTime = TimeSpan.FromSeconds(0L);
    else
      this.AddComp<ActiveGeneratorRevvingComponent>(uid, new ActiveGeneratorRevvingComponent());
  }

  public bool StopAutoRevving(EntityUid uid) => this.RemComp<ActiveGeneratorRevvingComponent>(uid);

  private bool StartGenerator(EntityUid uid)
  {
    AutoGeneratorStartedEvent args = new AutoGeneratorStartedEvent();
    this.RaiseLocalEvent<AutoGeneratorStartedEvent>(uid, ref args);
    return args.Started;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveGeneratorRevvingComponent, PortableGeneratorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveGeneratorRevvingComponent, PortableGeneratorComponent>();
    EntityUid uid;
    ActiveGeneratorRevvingComponent comp1;
    PortableGeneratorComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      comp1.CurrentTime += TimeSpan.FromSeconds((double) frameTime);
      this.Dirty(uid, (IComponent) comp1);
      if (!(comp1.CurrentTime < comp2.StartTime) && this.StartGenerator(uid))
        this.StopAutoRevving(uid);
    }
  }
}
