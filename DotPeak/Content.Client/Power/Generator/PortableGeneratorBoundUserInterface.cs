// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.Generator.PortableGeneratorBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power.Generator;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Power.Generator;

public sealed class PortableGeneratorBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  private GeneratorWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindowCenteredLeft<GeneratorWindow>((BoundUserInterface) this);
    this._window.SetEntity(this.Owner);
    this._window.OnState += (Action<bool>) (args =>
    {
      if (args)
        this.Start();
      else
        this.Stop();
    });
    this._window.OnPower += new Action<int>(this.SetTargetPower);
    this._window.OnEjectFuel += new Action(this.EjectFuel);
    this._window.OnSwitchOutput += new Action(this.SwitchOutput);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is PortableGeneratorComponentBuiState state1))
      return;
    this._window?.Update(state1);
  }

  public void SetTargetPower(int target)
  {
    this.SendMessage((BoundUserInterfaceMessage) new PortableGeneratorSetTargetPowerMessage(target));
  }

  public void Start()
  {
    this.SendMessage((BoundUserInterfaceMessage) new PortableGeneratorStartMessage());
  }

  public void Stop()
  {
    this.SendMessage((BoundUserInterfaceMessage) new PortableGeneratorStopMessage());
  }

  public void SwitchOutput()
  {
    this.SendMessage((BoundUserInterfaceMessage) new PortableGeneratorSwitchOutputMessage());
  }

  public void EjectFuel()
  {
    this.SendMessage((BoundUserInterfaceMessage) new PortableGeneratorEjectFuelMessage());
  }
}
