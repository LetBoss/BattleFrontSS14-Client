// Decompiled with JetBrains decompiler
// Type: Content.Client.Ensnaring.EnsnareableSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Ensnaring;
using Content.Shared.Ensnaring.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Ensnaring;

public sealed class EnsnareableSystem : SharedEnsnareableSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EnsnareableComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<EnsnareableComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  protected override void OnEnsnareInit(Entity<EnsnareableComponent> ent, ref ComponentInit args)
  {
    base.OnEnsnareInit(ent, ref args);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(ent.Owner, ref spriteComponent))
      return;
    this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), (Enum) EnsnaredVisualLayers.Ensnared);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    EnsnareableComponent component,
    ref AppearanceChangeEvent args)
  {
    int num;
    bool flag;
    if (args.Sprite == null || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) EnsnaredVisualLayers.Ensnared, ref num, false) || !this._appearance.TryGetData<bool>(uid, (Enum) EnsnareableVisuals.IsEnsnared, ref flag, args.Component) || component.Sprite == null)
      return;
    this._sprite.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, new ResPath(component.Sprite), new RSI.StateId?());
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(component.State));
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, flag);
  }
}
