// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IEntityNetworkManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IEntityNetworkManager
{
  event EventHandler<object> ReceivedSystemMessage;

  void SetupNetworking();

  void SendSystemNetworkMessage(EntityEventArgs message, bool recordReplay = true);

  void SendSystemNetworkMessage(EntityEventArgs message, uint sequence)
  {
    throw new NotSupportedException();
  }

  void SendSystemNetworkMessage(EntityEventArgs message, INetChannel channel);
}
