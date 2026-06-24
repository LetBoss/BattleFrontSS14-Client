// Decompiled with JetBrains decompiler
// Type: Content.Client.Clothing.Systems.ChameleonClothingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.PDA;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Clothing.Systems;

public sealed class ChameleonClothingSystem : SharedChameleonClothingSystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChameleonClothingComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<ChameleonClothingComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(HandleState)), (Type[]) null, (Type[]) null);
    this.PrepareAllVariants();
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnProtoReloaded), (Type[]) null, (Type[]) null);
  }

  private void OnProtoReloaded(PrototypesReloadedEventArgs args)
  {
    if (!args.WasModified<EntityPrototype>())
      return;
    this.PrepareAllVariants();
  }

  private void HandleState(
    EntityUid uid,
    ChameleonClothingComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateVisuals(uid, component);
  }

  protected override void UpdateSprite(EntityUid uid, EntityPrototype proto)
  {
    base.UpdateSprite(uid, proto);
    SpriteComponent spriteComponent1;
    SpriteComponent spriteComponent2;
    if (this.TryComp<SpriteComponent>(uid, ref spriteComponent1) && proto.TryGetComponent<SpriteComponent>(ref spriteComponent2, this.Factory))
      this._sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((EntityUid.Invalid, spriteComponent2)), Entity<SpriteComponent>.op_Implicit((uid, spriteComponent1)));
    PdaBorderColorComponent borderColorComponent1;
    PdaBorderColorComponent borderColorComponent2;
    if (!this.TryComp<PdaBorderColorComponent>(uid, ref borderColorComponent1) || !proto.TryGetComponent<PdaBorderColorComponent>(ref borderColorComponent2, this.Factory))
      return;
    borderColorComponent1.BorderColor = borderColorComponent2.BorderColor;
    borderColorComponent1.AccentHColor = borderColorComponent2.AccentHColor;
    borderColorComponent1.AccentVColor = borderColorComponent2.AccentVColor;
  }
}
