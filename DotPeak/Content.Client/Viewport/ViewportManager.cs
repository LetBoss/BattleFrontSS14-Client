// Decompiled with JetBrains decompiler
// Type: Content.Client.Viewport.ViewportManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Viewport;

public sealed class ViewportManager
{
  [Dependency]
  private IConfigurationManager _cfg;
  private readonly List<MainViewport> _viewports = new List<MainViewport>();

  public void Initialize()
  {
    this._cfg.OnValueChanged<bool>(CCVars.ViewportStretch, (Action<bool>) (_ => this.UpdateCfg()), false);
    this._cfg.OnValueChanged<int>(CCVars.ViewportSnapToleranceClip, (Action<int>) (_ => this.UpdateCfg()), false);
    this._cfg.OnValueChanged<int>(CCVars.ViewportSnapToleranceMargin, (Action<int>) (_ => this.UpdateCfg()), false);
    this._cfg.OnValueChanged<bool>(CCVars.ViewportScaleRender, (Action<bool>) (_ => this.UpdateCfg()), false);
    this._cfg.OnValueChanged<int>(CCVars.ViewportFixedScaleFactor, (Action<int>) (_ => this.UpdateCfg()), false);
  }

  private void UpdateCfg() => this._viewports.ForEach((Action<MainViewport>) (v => v.UpdateCfg()));

  public void AddViewport(MainViewport vp) => this._viewports.Add(vp);

  public void RemoveViewport(MainViewport vp) => this._viewports.Remove(vp);
}
