// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Viewport.ViewportUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared.CCVar;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Viewport;

public sealed class ViewportUIController : UIController
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IPlayerManager _playerMan;
  [Dependency]
  private IEntityManager _entMan;
  [Dependency]
  private IConfigurationManager _configurationManager;
  public static readonly Vector2i ViewportSize = Vector2i.op_Implicit((672, 480));
  public const int ViewportHeight = 15;

  private MainViewport? Viewport => this.UIManager.ActiveScreen?.GetWidget<MainViewport>();

  public virtual void Initialize()
  {
    this._configurationManager.OnValueChanged<int>(CCVars.ViewportMinimumWidth, (Action<int>) (_ => this.UpdateViewportRatio()), false);
    this._configurationManager.OnValueChanged<int>(CCVars.ViewportMaximumWidth, (Action<int>) (_ => this.UpdateViewportRatio()), false);
    this._configurationManager.OnValueChanged<int>(CCVars.ViewportWidth, (Action<int>) (_ => this.UpdateViewportRatio()), false);
    this._configurationManager.OnValueChanged<bool>(CCVars.ViewportVerticalFit, (Action<bool>) (_ => this.UpdateViewportRatio()), false);
    this.UIManager.GetUIController<GameplayStateLoadController>().OnScreenLoad += new Action(this.OnScreenLoad);
  }

  private void OnScreenLoad() => this.ReloadViewport();

  private void UpdateViewportRatio()
  {
    if (this.Viewport == null)
      return;
    int cvar1 = this._configurationManager.GetCVar<int>(CCVars.ViewportMinimumWidth);
    int cvar2 = this._configurationManager.GetCVar<int>(CCVars.ViewportMaximumWidth);
    int num = this._configurationManager.GetCVar<int>(CCVars.ViewportWidth);
    if ((!this._configurationManager.GetCVar<bool>(CCVars.ViewportVerticalFit) ? 0 : (this._configurationManager.GetCVar<bool>(CCVars.ViewportStretch) ? 1 : 0)) != 0)
      num = cvar2;
    else if (num < cvar1 || num > cvar2)
      num = CCVars.ViewportWidth.DefaultValue;
    this.Viewport.Viewport.ViewportSize = Vector2i.op_Implicit((32 /*0x20*/ * num, 480));
    this.Viewport.UpdateCfg();
  }

  public void ReloadViewport()
  {
    if (this.Viewport == null)
      return;
    this.UpdateViewportRatio();
    this.Viewport.Viewport.HorizontalExpand = true;
    this.Viewport.Viewport.VerticalExpand = true;
    this._eyeManager.MainViewport = (IViewportControl) this.Viewport.Viewport;
  }

  public virtual void FrameUpdate(FrameEventArgs e)
  {
    if (this.Viewport == null)
      return;
    base.FrameUpdate(e);
    this.Viewport.Viewport.Eye = this._eyeManager.CurrentEye;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerMan).LocalEntity;
    if (MapCoordinates.op_Inequality(this._eyeManager.CurrentEye.Position, new MapCoordinates()) || !localEntity.HasValue)
      return;
    EyeComponent eyeComponent;
    this._entMan.TryGetComponent<EyeComponent>(localEntity, ref eyeComponent);
    if (eyeComponent?.Eye == this._eyeManager.CurrentEye && MapId.op_Equality(this._entMan.GetComponent<TransformComponent>(localEntity.Value).MapID, MapId.Nullspace))
      return;
    this.Log.Warning($"Main viewport's eye is in nullspace (main eye is null?). Attached entity: {this._entMan.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(localEntity.Value))}. Entity has eye comp: {eyeComponent != null}");
  }
}
