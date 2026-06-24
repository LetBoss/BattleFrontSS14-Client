// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.UI.TransferAmountBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Chemistry.UI;

public sealed class TransferAmountBoundUserInterface : BoundUserInterface
{
  private IEntityManager _entManager;
  private EntityUid _owner;
  [Robust.Shared.ViewVariables.ViewVariables]
  private TransferAmountWindow? _window;

  public TransferAmountBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._owner = owner;
    this._entManager = IoCManager.Resolve<IEntityManager>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<TransferAmountWindow>((BoundUserInterface) this);
    SolutionTransferComponent transferComponent;
    if (this._entManager.TryGetComponent<SolutionTransferComponent>(this._owner, ref transferComponent))
    {
      TransferAmountWindow window = this._window;
      FixedPoint2 fixedPoint2 = transferComponent.MinimumTransferAmount;
      int min = fixedPoint2.Int();
      fixedPoint2 = transferComponent.MaximumTransferAmount;
      int max = fixedPoint2.Int();
      window.SetBounds(min, max);
    }
    ((BaseButton) this._window.ApplyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      int result;
      if (!int.TryParse(this._window.AmountLineEdit.Text, out result))
        return;
      this.SendMessage((BoundUserInterfaceMessage) new TransferAmountSetValueMessage(FixedPoint2.New(result)));
      ((BaseWindow) this._window).Close();
    });
  }
}
