// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgRay
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Network.Messages;

[NetSerializable]
[Serializable]
public sealed class MsgRay : EntityEventArgs
{
  public Vector2 RayOrigin;
  public Vector2 RayHit;
  public bool DidHit;
  public MapId Map;
}
