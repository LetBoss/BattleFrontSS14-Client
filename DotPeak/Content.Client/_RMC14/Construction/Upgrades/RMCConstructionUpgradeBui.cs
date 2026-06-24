// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Construction.Upgrades.RMCConstructionUpgradeBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Construction.Upgrades;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Construction.Upgrades;

public sealed class RMCConstructionUpgradeBui : BoundUserInterface
{
  [Dependency]
  private IClyde _displayManager;
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IPrototypeManager _prototypes;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private RMCConstructionUpgradeMenu? _menu;

  public RMCConstructionUpgradeBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<RMCConstructionUpgradeBui>(this);
    this._sprite = this.EntMan.System<SpriteSystem>();
    this._transform = this.EntMan.System<TransformSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<RMCConstructionUpgradeMenu>((BoundUserInterface) this);
    RMCConstructionUpgradeTargetComponent upgradeTargetComponent;
    if (this.EntMan.TryGetComponent<RMCConstructionUpgradeTargetComponent>(this.Owner, ref upgradeTargetComponent))
    {
      EntProtoId[] upgrades = upgradeTargetComponent.Upgrades;
      if (upgrades != null)
      {
        foreach (EntProtoId entProtoId in upgrades)
        {
          EntProtoId upgradeId = entProtoId;
          EntityPrototype entityPrototype;
          if (this._prototypes.TryIndex(upgradeId, ref entityPrototype))
          {
            RadialMenuTextureButton menuTextureButton1 = new RadialMenuTextureButton();
            ((Control) menuTextureButton1).StyleClasses.Add("RadialMenuButton");
            ((Control) menuTextureButton1).SetSize = new Vector2(64f, 64f);
            ((Control) menuTextureButton1).ToolTip = entityPrototype.Name;
            RadialMenuTextureButton menuTextureButton2 = menuTextureButton1;
            TextureRect textureRect1 = new TextureRect();
            ((Control) textureRect1).VerticalAlignment = (Control.VAlignment) 2;
            ((Control) textureRect1).HorizontalAlignment = (Control.HAlignment) 2;
            textureRect1.Texture = this._sprite.GetPrototypeIcon(entityPrototype).GetFrame((RsiDirection) 0, 0);
            textureRect1.TextureScale = new Vector2(2f, 2f);
            TextureRect textureRect2 = textureRect1;
            ((BaseButton) menuTextureButton2).OnButtonDown += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCConstructionUpgradeBuiMsg(upgradeId)));
            ((Control) menuTextureButton2).AddChild((Control) textureRect2);
            ((Control) this._menu.Upgrades).AddChild((Control) menuTextureButton2);
          }
        }
      }
    }
    if (this.EntMan.Deleted(this.Owner))
      return;
    Vector2i screenSize = this._displayManager.ScreenSize;
    this._menu.OpenCenteredAt(this._eye.WorldToScreen(((SharedTransformSystem) this._transform).GetMapCoordinates(this.Owner, (TransformComponent) null).Position) / Vector2i.op_Implicit(screenSize));
  }
}
