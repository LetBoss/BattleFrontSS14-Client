// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.Controls.GuidebookRichPrototypeLink
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Guidebook.RichText;
using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Guidebook.Controls;

public sealed class GuidebookRichPrototypeLink : Control, IPrototypeLinkControl
{
  private static readonly ISawmill Sawmill = Logger.GetSawmill("guidebook.links");
  private bool _linkActive;
  private FormattedMessage? _message;
  private readonly RichTextLabel _richTextLabel;

  public void EnablePrototypeLink()
  {
    if (this._message == null)
      return;
    this._linkActive = true;
    this.DefaultCursorShape = (Control.CursorShape) 3;
    this._richTextLabel.SetMessage(this._message, (Type[]) null, new Color?(TextLinkTag.LinkColor));
  }

  public GuidebookRichPrototypeLink()
  {
    this.MouseFilter = (Control.MouseFilterMode) 1;
    this.OnKeyBindDown += new Action<GUIBoundKeyEventArgs>(this.HandleClick);
    this._richTextLabel = new RichTextLabel();
    this.AddChild((Control) this._richTextLabel);
  }

  public void SetMessage(FormattedMessage message)
  {
    this._message = message;
    this._richTextLabel.SetMessage(this._message, new Color?());
  }

  public IPrototype? LinkedPrototype { get; set; }

  private void HandleClick(GUIBoundKeyEventArgs args)
  {
    if (!this._linkActive || BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
      return;
    IAnchorClickHandler result;
    if (this.TryGetParentHandler<IAnchorClickHandler>(out result))
    {
      result.HandleAnchor((IPrototypeLinkControl) this);
      ((BoundKeyEventArgs) args).Handle();
    }
    else
      GuidebookRichPrototypeLink.Sawmill.Warning("Warning! No valid IAnchorClickHandler found.");
  }
}
