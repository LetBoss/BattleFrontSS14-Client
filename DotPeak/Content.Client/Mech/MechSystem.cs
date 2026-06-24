// Decompiled with JetBrains decompiler
// Type: Content.Client.Mech.MechSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Mech;

public sealed class MechSystem : SharedMechSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MechComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<MechComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChanged)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChanged(
    EntityUid uid,
    MechComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null || !this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) MechVisualLayers.Base))
      return;
    string str = component.BaseState;
    Content.Shared.DrawDepth.DrawDepth drawDepth = Content.Shared.DrawDepth.DrawDepth.Mobs;
    bool flag1;
    if (((component.BrokenState == null ? 0 : (this._appearance.TryGetData<bool>(uid, (Enum) MechVisuals.Broken, ref flag1, args.Component) ? 1 : 0)) & (flag1 ? 1 : 0)) != 0)
    {
      str = component.BrokenState;
      drawDepth = Content.Shared.DrawDepth.DrawDepth.SmallMobs;
    }
    else
    {
      bool flag2;
      if (((component.OpenState == null ? 0 : (this._appearance.TryGetData<bool>(uid, (Enum) MechVisuals.Open, ref flag2, args.Component) ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
      {
        str = component.OpenState;
        drawDepth = Content.Shared.DrawDepth.DrawDepth.SmallMobs;
      }
    }
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) MechVisualLayers.Base, RSI.StateId.op_Implicit(str));
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (int) drawDepth);
  }
}
