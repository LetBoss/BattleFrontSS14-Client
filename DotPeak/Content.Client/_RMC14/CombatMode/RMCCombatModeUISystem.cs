// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.CombatMode.RMCCombatModeUISystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Emplacements;
using Content.Client.CombatMode;
using Content.Client.Hands.Systems;
using Content.Shared._RMC14.CombatMode;
using Content.Shared.CCVar;
using Content.Shared.Wieldable.Components;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

#nullable enable
namespace Content.Client._RMC14.CombatMode;

public sealed class RMCCombatModeUISystem : EntitySystem
{
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private CombatModeSystem _combatMode;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private HandsSystem _hands;
  [Dependency]
  private RMCCombatModeSystem _rmcCombatMode;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private RMCWeaponControllerSystem _rmcSharedWeaponController;
  private bool _crosshairsEnabled;
  private ICursor? _crosshairCursor;

  public virtual void Initialize()
  {
    base.Initialize();
    EntitySystemSubscriptionExt.CVar<bool>(this.Subs, this._config, CCVars.CombatModeIndicatorsPointShow, (Action<bool>) (v => this._crosshairsEnabled = v), true);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    if (!(this._ui.CurrentlyHovered is IViewportControl))
      return;
    if (!this._crosshairsEnabled || !this._combatMode.IsInCombatMode())
    {
      this._ui.CurrentlyHovered.CustomCursorShape = (ICursor) null;
    }
    else
    {
      EntityUid? nullable = this._hands.GetActiveHandEntity();
      EntityUid? weapon;
      if (this._rmcSharedWeaponController.TryGetControllingWeapon(out weapon))
        nullable = weapon;
      if (!nullable.HasValue || this._rmcCombatMode.GetCrosshair(Entity<WieldedCrosshairComponent, WieldableComponent>.op_Implicit(nullable.Value)) == null)
      {
        this._ui.CurrentlyHovered.CustomCursorShape = (ICursor) null;
      }
      else
      {
        if (this._crosshairCursor == null)
          this._crosshairCursor = this._clyde.CreateCursor(new Image<Rgba32>(1, 1), Vector2i.Zero);
        this._ui.CurrentlyHovered.CustomCursorShape = this._crosshairCursor;
      }
    }
  }
}
