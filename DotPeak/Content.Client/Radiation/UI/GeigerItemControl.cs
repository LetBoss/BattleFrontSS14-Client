// Decompiled with JetBrains decompiler
// Type: Content.Client.Radiation.UI.GeigerItemControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Systems;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client.Radiation.UI;

public sealed class GeigerItemControl : Control
{
  private readonly GeigerComponent _component;
  private readonly RichTextLabel _label;

  public GeigerItemControl(GeigerComponent component)
  {
    this._component = component;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this.AddChild((Control) this._label);
    this.Update();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._component.UiUpdateNeeded)
      return;
    this.Update();
  }

  private void Update()
  {
    string markup;
    if (this._component.IsEnabled)
    {
      Color color = SharedGeigerSystem.LevelToColor(this._component.DangerLevel);
      markup = Loc.GetString("geiger-item-control-status", new (string, object)[2]
      {
        ("rads", (object) this._component.CurrentRadiation.ToString("N1")),
        ("color", (object) color)
      });
    }
    else
      markup = Loc.GetString("geiger-item-control-disabled");
    this._label.SetMarkup(markup);
    this._component.UiUpdateNeeded = false;
  }
}
