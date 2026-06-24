// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.ServerBoundUserInterfaceMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Player;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class ServerBoundUserInterfaceMessage
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public BoundUserInterfaceMessage Message { get; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public ICommonSession Session { get; }

  public ServerBoundUserInterfaceMessage(BoundUserInterfaceMessage message, ICommonSession session)
  {
    this.Message = message;
    this.Session = session;
  }
}
