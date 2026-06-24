// Decompiled with JetBrains decompiler
// Type: Content.Client.Forensics.ForensicScannerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Forensics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Threading;

#nullable enable
namespace Content.Client.Forensics;

public sealed class ForensicScannerBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Robust.Shared.ViewVariables.ViewVariables]
  private ForensicScannerMenu? _window;
  [Robust.Shared.ViewVariables.ViewVariables]
  private TimeSpan _printCooldown;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<ForensicScannerMenu>((BoundUserInterface) this);
    ((BaseButton) this._window.Print).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.Print());
    ((BaseButton) this._window.Clear).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.Clear());
  }

  private void Print()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ForensicScannerPrintMessage());
    if (this._window != null)
      this._window.UpdatePrinterState(true);
    Timer.Spawn(this._printCooldown, (Action) (() =>
    {
      if (this._window == null)
        return;
      this._window.UpdatePrinterState(false);
    }), new CancellationToken());
  }

  private void Clear()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ForensicScannerClearMessage());
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is ForensicScannerBoundUserInterfaceState msg))
      return;
    this._printCooldown = msg.PrintCooldown;
    if (msg.PrintReadyAt > this._gameTiming.CurTime)
      Timer.Spawn(msg.PrintReadyAt - this._gameTiming.CurTime, (Action) (() =>
      {
        if (this._window == null)
          return;
        this._window.UpdatePrinterState(false);
      }), new CancellationToken());
    this._window.UpdateState(msg);
  }
}
