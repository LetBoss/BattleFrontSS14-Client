// Decompiled with JetBrains decompiler
// Type: Content.Client.Crayon.UI.CrayonBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Crayon;
using Content.Shared.Decals;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.Crayon.UI;

public sealed class CrayonBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IPrototypeManager _protoManager;
  [Robust.Shared.ViewVariables.ViewVariables]
  private CrayonWindow? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<CrayonWindow>((BoundUserInterface) this);
    this._menu.OnColorSelected += new Action<Color>(this.SelectColor);
    this._menu.OnSelected += new Action<string>(this.Select);
    this.PopulateCrayons();
  }

  private void PopulateCrayons()
  {
    this._menu?.Populate(this._protoManager.EnumeratePrototypes<DecalPrototype>().Where<DecalPrototype>((Func<DecalPrototype, bool>) (x => x.Tags.Contains("crayon"))).ToList<DecalPrototype>());
  }

  public virtual void OnProtoReload(PrototypesReloadedEventArgs args)
  {
    base.OnProtoReload(args);
    if (!args.WasModified<DecalPrototype>())
      return;
    this.PopulateCrayons();
  }

  protected virtual void ReceiveMessage(BoundUserInterfaceMessage message)
  {
    base.ReceiveMessage(message);
    if (this._menu == null || !(message is CrayonUsedMessage crayonUsedMessage))
      return;
    this._menu.AdvanceState(crayonUsedMessage.DrawnDecal);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._menu?.UpdateState((CrayonBoundUserInterfaceState) state);
  }

  public void Select(string state)
  {
    this.SendMessage((BoundUserInterfaceMessage) new CrayonSelectMessage(state));
  }

  public void SelectColor(Color color)
  {
    this.SendMessage((BoundUserInterfaceMessage) new CrayonColorMessage(color));
  }
}
