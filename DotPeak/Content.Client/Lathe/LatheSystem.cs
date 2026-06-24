// Decompiled with JetBrains decompiler
// Type: Content.Client.Lathe.LatheSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power;
using Content.Shared.Lathe;
using Content.Shared.Power;
using Content.Shared.Research.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Lathe;

public sealed class LatheSystem : SharedLatheSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LatheComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<LatheComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    LatheComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag1;
    int num1;
    if (this._appearance.TryGetData<bool>(uid, (Enum) LatheVisuals.IsRunning, ref flag1, args.Component) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) LatheVisualLayers.IsRunning, ref num1, false) && component.RunningState != null && component.IdleState != null)
    {
      string str = flag1 ? component.RunningState : component.IdleState;
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num1, RSI.StateId.op_Implicit(str));
    }
    bool flag2;
    int num2;
    if (!this._appearance.TryGetData<bool>(uid, (Enum) PowerDeviceVisuals.Powered, ref flag2, args.Component) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PowerDeviceVisualLayers.Powered, ref num2, false))
      return;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, flag2);
    if (component.UnlitIdleState == null || component.UnlitRunningState == null)
      return;
    string str1 = flag1 ? component.UnlitRunningState : component.UnlitIdleState;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num2, RSI.StateId.op_Implicit(str1));
  }

  protected override bool HasRecipe(
    EntityUid uid,
    LatheRecipePrototype recipe,
    LatheComponent component)
  {
    return true;
  }
}
