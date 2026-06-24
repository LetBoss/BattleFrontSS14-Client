// Decompiled with JetBrains decompiler
// Type: Content.Client.SubFloor.SubFloorHideSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Sandbox;
using Content.Shared.SubFloor;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.SubFloor;

public sealed class SubFloorHideSystem : SharedSubFloorHideSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private IUserInterfaceManager _ui;
  private bool _showAll;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool ShowAll
  {
    get => this._showAll;
    set
    {
      if (this._showAll == value)
        return;
      this._showAll = value;
      this._ui.GetUIController<SandboxUIController>().SetToggleSubfloors(value);
      this.RaiseNetworkEvent((EntityEventArgs) new SharedSubFloorHideSystem.ShowSubfloorRequestEvent()
      {
        Value = value
      });
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SubFloorHideComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SubFloorHideComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChanged)), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<SharedSubFloorHideSystem.ShowSubfloorRequestEvent>(new EntityEventHandler<SharedSubFloorHideSystem.ShowSubfloorRequestEvent>(this.OnRequestReceived), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<LocalPlayerDetachedEvent>(new EntityEventHandler<LocalPlayerDetachedEvent>(this.OnPlayerDetached), (Type[]) null, (Type[]) null);
  }

  private void OnPlayerDetached(LocalPlayerDetachedEvent ev) => this.ShowAll = false;

  private void OnRequestReceived(
    SharedSubFloorHideSystem.ShowSubfloorRequestEvent ev)
  {
    this.UpdateAll();
  }

  private void OnAppearanceChanged(
    EntityUid uid,
    SubFloorHideComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag1;
    this._appearance.TryGetData<bool>(uid, (Enum) SubFloorVisuals.Covered, ref flag1, args.Component);
    bool flag2;
    this._appearance.TryGetData<bool>(uid, (Enum) SubFloorVisuals.ScannerRevealed, ref flag2, args.Component);
    bool flag3 = flag2 & !this.ShowAll;
    bool flag4 = ((!flag1 ? 1 : (this.ShowAll ? 1 : 0)) | (flag3 ? 1 : 0)) != 0;
    foreach (ISpriteLayer allLayer in args.Sprite.AllLayers)
      allLayer.Visible = flag4;
    bool flag5 = false;
    foreach (Enum visibleLayer in component.VisibleLayers)
    {
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), visibleLayer, ref num, false))
      {
        ISpriteLayer ispriteLayer = args.Sprite[num];
        ispriteLayer.Visible = true;
        Color color = ispriteLayer.Color;
        ispriteLayer.Color = ((Color) ref color).WithAlpha(1f);
        flag5 = true;
      }
    }
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), flag5 | flag4);
    if (this.ShowAll)
    {
      SubFloorHideComponent floorHideComponent = component;
      floorHideComponent.OriginalDrawDepth.GetValueOrDefault();
      if (!floorHideComponent.OriginalDrawDepth.HasValue)
      {
        int drawDepth = args.Sprite.DrawDepth;
        floorHideComponent.OriginalDrawDepth = new int?(drawDepth);
      }
      this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 10);
    }
    else if (flag3)
    {
      if (component.OriginalDrawDepth.HasValue)
        return;
      component.OriginalDrawDepth = new int?(args.Sprite.DrawDepth);
      int num = -9;
      this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), args.Sprite.DrawDepth - (num - 1));
    }
    else
    {
      if (!component.OriginalDrawDepth.HasValue)
        return;
      this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.OriginalDrawDepth.Value);
      component.OriginalDrawDepth = new int?();
    }
  }

  private void UpdateAll()
  {
    AllEntityQueryEnumerator<SubFloorHideComponent, AppearanceComponent> entityQueryEnumerator = this.AllEntityQuery<SubFloorHideComponent, AppearanceComponent>();
    EntityUid entityUid;
    SubFloorHideComponent floorHideComponent;
    AppearanceComponent appearanceComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref floorHideComponent, ref appearanceComponent))
      this._appearance.QueueUpdate(entityUid, appearanceComponent);
  }
}
