// Decompiled with JetBrains decompiler
// Type: Content.Client.Computer.IComputerWindow`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Client.Computer;

public interface IComputerWindow<TState>
{
  void SetupComputerWindow(ComputerBoundUserInterfaceBase cb)
  {
  }

  void UpdateState(TState state)
  {
  }

  void ReceiveMessage(BoundUserInterfaceMessage message)
  {
  }
}
