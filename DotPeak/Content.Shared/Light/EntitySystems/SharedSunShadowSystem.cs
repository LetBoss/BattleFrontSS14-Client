// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.SharedSunShadowSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

#nullable enable
namespace Content.Shared.Light.EntitySystems;

public abstract class SharedSunShadowSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _random;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SunShadowCycleComponent, MapInitEvent>(new EntityEventRefHandler<SunShadowCycleComponent, MapInitEvent>(this.OnCycleMapInit));
    this.SubscribeLocalEvent<SunShadowCycleComponent, LightCycleOffsetEvent>(new EntityEventRefHandler<SunShadowCycleComponent, LightCycleOffsetEvent>(this.OnCycleOffset));
  }

  private void OnCycleOffset(Entity<SunShadowCycleComponent> ent, ref LightCycleOffsetEvent args)
  {
    ent.Comp.Offset = args.Offset;
    this.Dirty<SunShadowCycleComponent>(ent);
  }

  private void OnCycleMapInit(Entity<SunShadowCycleComponent> ent, ref MapInitEvent args)
  {
    LightCycleComponent comp;
    if (this.TryComp<LightCycleComponent>(ent.Owner, out comp))
    {
      ent.Comp.Duration = comp.Duration;
      ent.Comp.Offset = comp.Offset;
    }
    else
      ent.Comp.Offset = this._random.Next(ent.Comp.Duration);
    this.Dirty<SunShadowCycleComponent>(ent);
  }
}
