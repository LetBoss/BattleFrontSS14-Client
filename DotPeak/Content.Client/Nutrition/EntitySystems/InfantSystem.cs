// Decompiled with JetBrains decompiler
// Type: Content.Client.Nutrition.EntitySystems.InfantSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Nutrition.AnimalHusbandry;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Nutrition.EntitySystems;

public sealed class InfantSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InfantComponent, ComponentStartup>(new ComponentEventHandler<InfantComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InfantComponent, ComponentShutdown>(new ComponentEventHandler<InfantComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, InfantComponent component, ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    component.DefaultScale = spriteComponent.Scale;
    this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component.VisualScale);
  }

  private void OnShutdown(EntityUid uid, InfantComponent component, ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component.DefaultScale);
  }
}
