// Decompiled with JetBrains decompiler
// Type: Content.Client.Remote.UI.DoorRemoteStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared.Remotes.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client.Remote.UI;

public sealed class DoorRemoteStatusControl : Control
{
  private readonly Entity<DoorRemoteComponent> _entity;
  private readonly RichTextLabel _label;
  private OperatingMode PrevOperatingMode = OperatingMode.placeholderForUiUpdates;

  public DoorRemoteStatusControl(Entity<DoorRemoteComponent> entity)
  {
    this._entity = entity;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this.AddChild((Control) this._label);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (this.PrevOperatingMode == this._entity.Comp.Mode)
      return;
    this.PrevOperatingMode = this._entity.Comp.Mode;
    string str;
    switch (this._entity.Comp.Mode)
    {
      case OperatingMode.OpenClose:
        str = "door-remote-open-close-text";
        break;
      case OperatingMode.ToggleBolts:
        str = "door-remote-toggle-bolt-text";
        break;
      case OperatingMode.ToggleEmergencyAccess:
        str = "door-remote-emergency-access-text";
        break;
      default:
        str = "door-remote-invalid-text";
        break;
    }
    this._label.SetMarkup(Loc.GetString("door-remote-mode-label", new (string, object)[1]
    {
      ("modeString", (object) Loc.GetString(str))
    }));
  }
}
