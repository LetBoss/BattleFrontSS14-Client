// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.RichText.TextLinkTag
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Input;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.Guidebook.RichText;

public sealed class TextLinkTag : IMarkupTagHandler
{
  private static readonly ISawmill Sawmill = Logger.GetSawmill("guidebook.textlink");

  public static Color LinkColor => Color.CornflowerBlue;

  public string Name => "textlink";

  public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
  {
    string str;
    MarkupParameter markupParameter;
    string link;
    if (!((MarkupParameter) ref node.Value).TryGetString(ref str) || !node.Attributes.TryGetValue("link", out markupParameter) || !((MarkupParameter) ref markupParameter).TryGetString(ref link))
    {
      control = (Control) null;
      return false;
    }
    Label label = new Label();
    label.Text = str;
    ((Control) label).MouseFilter = (Control.MouseFilterMode) 0;
    label.FontColorOverride = new Color?(TextLinkTag.LinkColor);
    ((Control) label).DefaultCursorShape = (Control.CursorShape) 3;
    ((Control) label).OnMouseEntered += (Action<GUIMouseHoverEventArgs>) (_ => label.FontColorOverride = new Color?(Color.LightSkyBlue));
    ((Control) label).OnMouseExited += (Action<GUIMouseHoverEventArgs>) (_ => label.FontColorOverride = new Color?(Color.CornflowerBlue));
    ((Control) label).OnKeyBindDown += (Action<GUIBoundKeyEventArgs>) (args => this.OnKeybindDown(args, link, (Control) label));
    control = (Control) label;
    return true;
  }

  private void OnKeybindDown(GUIBoundKeyEventArgs args, string link, Control? control)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) || control == null)
      return;
    ILinkClickHandler result;
    if (control.TryGetParentHandler<ILinkClickHandler>(out result))
      result.HandleClick(link);
    else
      TextLinkTag.Sawmill.Warning("Warning! No valid ILinkClickHandler found.");
  }
}
