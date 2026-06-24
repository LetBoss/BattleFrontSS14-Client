// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Evolution.XenoDevolveBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Xenonids.UI;
using Content.Shared._RMC14.Xenonids.Evolution;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Evolution;

public sealed class XenoDevolveBui : BoundUserInterface
{
  [Dependency]
  private IPrototypeManager _prototype;
  private readonly SpriteSystem _sprite;
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoDevolveWindow? _window;

  public XenoDevolveBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._sprite = this.EntMan.System<SpriteSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<XenoDevolveWindow>((BoundUserInterface) this);
    XenoDevolveComponent devolveComponent;
    if (!this.EntMan.TryGetComponent<XenoDevolveComponent>(this.Owner, ref devolveComponent))
      return;
    foreach (EntProtoId entProtoId in devolveComponent.DevolvesTo)
    {
      EntProtoId devolvesTo = entProtoId;
      EntityPrototype entityPrototype;
      if (!this._prototype.TryIndex(devolvesTo, ref entityPrototype))
        break;
      XenoChoiceControl xenoChoiceControl = new XenoChoiceControl();
      xenoChoiceControl.Set(entityPrototype.Name, this._sprite.Frame0(entityPrototype));
      ((BaseButton) xenoChoiceControl.Button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoDevolveBuiMsg(devolvesTo));
        this.Close();
      });
      ((Control) this._window.DevolutionsContainer).AddChild((Control) xenoChoiceControl);
    }
  }
}
