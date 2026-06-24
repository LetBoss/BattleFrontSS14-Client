// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Player.ICommonSessionInternal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

#nullable enable
namespace Robust.Shared.Player;

internal interface ICommonSessionInternal : ICommonSession
{
  void SetStatus(SessionStatus status);

  void SetAttachedEntity(EntityUid? uid);

  void SetPing(short ping);

  void SetName(string name);

  void SetChannel(INetChannel channel);
}
