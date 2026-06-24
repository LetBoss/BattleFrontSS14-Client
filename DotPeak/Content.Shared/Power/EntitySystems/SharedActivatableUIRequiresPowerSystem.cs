// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.EntitySystems.SharedActivatableUIRequiresPowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Power.Components;
using Content.Shared.UserInterface;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Power.EntitySystems;

public abstract class SharedActivatableUIRequiresPowerSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActivatableUIRequiresPowerComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<ActivatableUIRequiresPowerComponent, ActivatableUIOpenAttemptEvent>(this.OnActivate));
  }

  protected abstract void OnActivate(
    Entity<ActivatableUIRequiresPowerComponent> ent,
    ref ActivatableUIOpenAttemptEvent args);
}
