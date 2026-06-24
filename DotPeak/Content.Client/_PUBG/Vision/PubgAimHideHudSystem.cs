// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Vision.PubgAimHideHudSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Systems.Hotbar.Widgets;
using Content.Shared._PUBG.Vision;
using Content.Shared.CombatMode;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

#nullable enable
namespace Content.Client._PUBG.Vision;

public sealed class PubgAimHideHudSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private SharedHandsSystem _hands;
  private bool _hidden;

  public virtual void FrameUpdate(float frameTime)
  {
    HotbarGui activeUiWidgetOrNull = this._ui.GetActiveUIWidgetOrNull<HotbarGui>();
    if (activeUiWidgetOrNull == null)
    {
      this._hidden = false;
    }
    else
    {
      bool flag = this.ShouldHide();
      if (flag == this._hidden)
        return;
      this._hidden = flag;
      if (flag)
      {
        ((Control) activeUiWidgetOrNull).Visible = false;
      }
      else
      {
        EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
        ((Control) activeUiWidgetOrNull).Visible = localEntity.HasValue && this.HasComp<HandsComponent>(localEntity.Value);
      }
    }
  }

  private bool ShouldHide()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CombatModeComponent combatModeComponent;
    EntityUid? nullable;
    PubgFocusViewComponent focusViewComponent;
    if (!localEntity.HasValue || !this.TryComp<CombatModeComponent>(localEntity.Value, ref combatModeComponent) || !combatModeComponent.IsInCombatMode || !this._hands.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((localEntity.Value, (HandsComponent) null)), out nullable) || !nullable.HasValue || !this.TryComp<PubgFocusViewComponent>(nullable.Value, ref focusViewComponent) || !focusViewComponent.Active)
      return false;
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    if (WindowId.op_Equality(mouseScreenPosition.Window, WindowId.Invalid))
      return false;
    int y = this._clyde.MainWindow.Size.Y;
    return (double) ((ScreenCoordinates) ref mouseScreenPosition).Y > (double) y / 2.0;
  }
}
