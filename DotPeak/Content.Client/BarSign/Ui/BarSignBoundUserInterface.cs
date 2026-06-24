// Decompiled with JetBrains decompiler
// Type: Content.Client.BarSign.Ui.BarSignBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.BarSign;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.BarSign.Ui;

public sealed class BarSignBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IPrototypeManager _prototype;
  private BarSignMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    ProtoId<BarSignPrototype>? current = (ProtoId<BarSignPrototype>?) EntityManagerExt.GetComponentOrNull<BarSignComponent>(this.EntMan, this.Owner)?.Current;
    this._menu = new BarSignMenu(!current.HasValue ? (BarSignPrototype) null : this._prototype.Index<BarSignPrototype>(current.GetValueOrDefault()), Content.Shared.BarSign.BarSignSystem.GetAllBarSigns(this._prototype).OrderBy<BarSignPrototype, string>((Func<BarSignPrototype, string>) (p => Loc.GetString(LocId.op_Implicit(p.Name)))).ToList<BarSignPrototype>());
    this._menu.OnSignSelected += (Action<string>) (id => this.SendMessage((BoundUserInterfaceMessage) new SetBarSignMessage(ProtoId<BarSignPrototype>.op_Implicit(id))));
    this._menu.OnClose += new Action(((BoundUserInterface) this).Close);
    this._menu.OpenCentered();
  }

  public void Update(ProtoId<BarSignPrototype>? sign)
  {
    BarSignPrototype newSign;
    if (!this._prototype.TryIndex<BarSignPrototype>(sign, ref newSign))
      return;
    this._menu?.UpdateState(newSign);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._menu)?.Orphan();
  }
}
