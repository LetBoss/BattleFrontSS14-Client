// Decompiled with JetBrains decompiler
// Type: Content.Client.Crayon.CrayonSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Items;
using Content.Client.Message;
using Content.Shared.Crayon;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Crayon;

public sealed class CrayonSystem : SharedCrayonSystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CrayonComponent, ComponentHandleState>(CrayonSystem.\u003C\u003EO.\u003C0\u003E__OnCrayonHandleState ?? (CrayonSystem.\u003C\u003EO.\u003C0\u003E__OnCrayonHandleState = new ComponentEventRefHandler<CrayonComponent, ComponentHandleState>((object) null, __methodptr(OnCrayonHandleState))), (Type[]) null, (Type[]) null);
    this.Subs.ItemStatus<CrayonComponent>((Func<Entity<CrayonComponent>, Control>) (ent => (Control) new CrayonSystem.StatusControl(Entity<CrayonComponent>.op_Implicit(ent))));
  }

  private static void OnCrayonHandleState(
    EntityUid uid,
    CrayonComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is CrayonComponentState current))
      return;
    component.Color = current.Color;
    component.SelectedState = current.State;
    component.Charges = current.Charges;
    component.Capacity = current.Capacity;
    component.UIUpdateNeeded = true;
  }

  private sealed class StatusControl : Control
  {
    private readonly CrayonComponent _parent;
    private readonly RichTextLabel _label;

    public StatusControl(CrayonComponent parent)
    {
      this._parent = parent;
      RichTextLabel richTextLabel = new RichTextLabel();
      ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
      this._label = richTextLabel;
      this.AddChild((Control) this._label);
      parent.UIUpdateNeeded = true;
    }

    protected virtual void FrameUpdate(FrameEventArgs args)
    {
      base.FrameUpdate(args);
      if (!this._parent.UIUpdateNeeded)
        return;
      this._parent.UIUpdateNeeded = false;
      this._label.SetMarkup(Loc.GetString("crayon-drawing-label", new (string, object)[4]
      {
        ("color", (object) this._parent.Color),
        ("state", (object) this._parent.SelectedState),
        ("charges", (object) this._parent.Charges),
        ("capacity", (object) this._parent.Capacity)
      }));
    }
  }
}
