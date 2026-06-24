// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Pheromones.XenoPheromonesBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Pheromones;

public sealed class XenoPheromonesBui : BoundUserInterface
{
  [Dependency]
  private IClyde _displayManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IEyeManager _eye;
  private readonly SpriteSystem _sprite;
  private readonly TransformSystem _transform;
  private readonly SharedXenoPheromonesSystem _pheros;
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoPheromonesMenu? _xenoPheromonesMenu;
  private const string HelpButtonTexture = "radial_help";

  public XenoPheromonesBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    IoCManager.InjectDependencies<XenoPheromonesBui>(this);
    this._sprite = this.EntMan.System<SpriteSystem>();
    this._transform = this.EntMan.System<TransformSystem>();
    this._pheros = this.EntMan.System<SharedXenoPheromonesSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._xenoPheromonesMenu = BoundUserInterfaceExt.CreateWindow<XenoPheromonesMenu>((BoundUserInterface) this);
    RadialContainer control = ((Control) this._xenoPheromonesMenu).FindControl<RadialContainer>("Main");
    if (this.EntMan.HasComponent<XenoComponent>(this.Owner))
    {
      TextureRect textureRect1 = new TextureRect();
      ((Control) textureRect1).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) textureRect1).HorizontalAlignment = (Control.HAlignment) 2;
      textureRect1.Texture = this._sprite.Frame0((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/radial.rsi"), "radial_help"));
      textureRect1.TextureScale = new Vector2(2f, 2f);
      TextureRect textureRect2 = textureRect1;
      RadialMenuTextureButton menuTextureButton1 = new RadialMenuTextureButton();
      ((Control) menuTextureButton1).StyleClasses.Add("RadialMenuButton");
      ((Control) menuTextureButton1).SetSize = new Vector2(64f, 64f);
      RadialMenuTextureButton menuTextureButton2 = menuTextureButton1;
      ((BaseButton) menuTextureButton2).OnButtonDown += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoPheromonesHelpButtonBuiMsg()));
      ((Control) menuTextureButton2).AddChild((Control) textureRect2);
      ((Control) control).AddChild((Control) menuTextureButton2);
      this.AddPheromonesButton(XenoPheromones.Frenzy, control, this.Owner);
      this.AddPheromonesButton(XenoPheromones.Warding, control, this.Owner);
      this.AddPheromonesButton(XenoPheromones.Recovery, control, this.Owner);
    }
    Vector2i screenSize = this._displayManager.ScreenSize;
    Vector2 vector2 = this._inputManager.MouseScreenPosition.Position / Vector2i.op_Implicit(screenSize);
    EyeComponent eyeComponent;
    if (this.EntMan.TryGetComponent<EyeComponent>(this.Owner, ref eyeComponent) && eyeComponent.Target.HasValue)
    {
      vector2 = this._eye.WorldToScreen(((SharedTransformSystem) this._transform).GetMapCoordinates(eyeComponent.Target.Value, (TransformComponent) null).Position) / Vector2i.op_Implicit(screenSize);
    }
    else
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      if (localEntity.HasValue)
        vector2 = this._eye.WorldToScreen(((SharedTransformSystem) this._transform).GetMapCoordinates(localEntity.GetValueOrDefault(), (TransformComponent) null).Position) / Vector2i.op_Implicit(screenSize);
    }
    this._xenoPheromonesMenu.OpenCenteredAt(vector2);
  }

  private void AddPheromonesButton(
    XenoPheromones pheromone,
    RadialContainer parent,
    EntityUid owner)
  {
    string lowerInvariant = pheromone.ToString().ToLowerInvariant();
    string str = this._pheros.GetPheroSuffix(Entity<XenoPheromonesComponent>.op_Implicit((owner, (XenoPheromonesComponent) null)));
    if (str != null)
      str = "_" + str;
    TextureRect textureRect1 = new TextureRect();
    ((Control) textureRect1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) textureRect1).HorizontalAlignment = (Control.HAlignment) 2;
    textureRect1.Texture = this._sprite.Frame0((SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("/Textures/_RMC14/Interface/xeno_pheromones.rsi"), lowerInvariant + str));
    textureRect1.TextureScale = new Vector2(2f, 2f);
    TextureRect textureRect2 = textureRect1;
    RadialMenuTextureButton menuTextureButton1 = new RadialMenuTextureButton();
    ((Control) menuTextureButton1).StyleClasses.Add("RadialMenuButton");
    ((Control) menuTextureButton1).SetSize = new Vector2(64f, 64f);
    ((Control) menuTextureButton1).ToolTip = lowerInvariant;
    RadialMenuTextureButton menuTextureButton2 = menuTextureButton1;
    ((BaseButton) menuTextureButton2).OnButtonDown += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoPheromonesChosenBuiMsg(pheromone)));
    ((Control) menuTextureButton2).AddChild((Control) textureRect2);
    ((Control) parent).AddChild((Control) menuTextureButton2);
  }
}
