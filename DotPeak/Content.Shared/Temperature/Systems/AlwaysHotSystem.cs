// Decompiled with JetBrains decompiler
// Type: Content.Shared.Temperature.Systems.AlwaysHotSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Temperature.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Temperature.Systems;

public sealed class AlwaysHotSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AlwaysHotComponent, IsHotEvent>(new EntityEventRefHandler<AlwaysHotComponent, IsHotEvent>(this.OnIsHot));
  }

  private void OnIsHot(Entity<AlwaysHotComponent> ent, ref IsHotEvent args) => args.IsHot = true;
}
