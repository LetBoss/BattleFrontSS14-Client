// Decompiled with JetBrains decompiler
// Type: Content.Client.Implants.UI.ImplanterStatusControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Client.UserInterface.Controls;
using Content.Shared.Implants.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client.Implants.UI;

public sealed class ImplanterStatusControl : Control
{
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly ImplanterComponent _parent;
  private readonly RichTextLabel _label;

  public ImplanterStatusControl(ImplanterComponent parent)
  {
    IoCManager.InjectDependencies<ImplanterStatusControl>(this);
    this._parent = parent;
    RichTextLabel richTextLabel = new RichTextLabel();
    ((Control) richTextLabel).StyleClasses.Add("ItemStatus");
    this._label = richTextLabel;
    ((Control) this._label).MaxWidth = 350f;
    ClipControl clipControl = new ClipControl();
    clipControl.Children.Add((Control) this._label);
    this.AddChild((Control) clipControl);
    this.Update();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._parent.UiUpdateNeeded)
      return;
    this.Update();
  }

  private void Update()
  {
    this._parent.UiUpdateNeeded = false;
    string str1;
    switch (this._parent.CurrentMode)
    {
      case ImplanterToggleMode.Inject:
        str1 = Loc.GetString("implanter-inject-text");
        break;
      case ImplanterToggleMode.Draw:
        str1 = Loc.GetString("implanter-draw-text");
        break;
      default:
        str1 = Loc.GetString("injector-invalid-injector-toggle-mode");
        break;
    }
    string str2 = str1;
    if (this._parent.CurrentMode == ImplanterToggleMode.Draw)
    {
      EntityPrototype entityPrototype;
      this._label.SetMarkup(Loc.GetString("implanter-label-draw", new (string, object)[2]
      {
        ("implantName", (object) (this._parent.DeimplantChosen.HasValue ? (this._prototype.TryIndex(this._parent.DeimplantChosen.Value, ref entityPrototype) ? entityPrototype.Name : Loc.GetString("implanter-empty-text")) : Loc.GetString("implanter-empty-text"))),
        ("modeString", (object) str2)
      }));
    }
    else
      this._label.SetMarkup(Loc.GetString("implanter-label-inject", new (string, object)[2]
      {
        ("implantName", (object) (this._parent.ImplanterSlot.HasItem ? this._parent.ImplantData.Item1 : Loc.GetString("implanter-empty-text"))),
        ("modeString", (object) str2)
      }));
  }
}
