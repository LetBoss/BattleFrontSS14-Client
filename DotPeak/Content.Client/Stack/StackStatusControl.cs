// Decompiled with JetBrains decompiler
// Type: Content.Client.Stack.StackStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared.Stacks;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client.Stack;

public sealed class StackStatusControl : Control
{
  private readonly StackComponent _parent;
  private readonly RichTextLabel _label;

  public StackStatusControl(StackComponent parent)
  {
    this._parent = parent;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    this._label.SetMarkup(Loc.GetString("comp-stack-status", new (string, object)[1]
    {
      ("count", (object) this._parent.Count)
    }));
    this.AddChild((Control) this._label);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._parent.UiUpdateNeeded)
      return;
    this._parent.UiUpdateNeeded = false;
    this._label.SetMarkup(Loc.GetString("comp-stack-status", new (string, object)[1]
    {
      ("count", (object) this._parent.Count)
    }));
  }
}
