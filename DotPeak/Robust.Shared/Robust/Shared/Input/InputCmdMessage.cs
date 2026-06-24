// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.InputCmdMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Robust.Shared.Input;

[NetSerializable]
[Serializable]
public abstract class InputCmdMessage : EntityEventArgs, IComparable<InputCmdMessage>
{
  public GameTick Tick { get; }

  public ushort SubTick { get; }

  public KeyFunctionId InputFunctionId { get; }

  public uint InputSequence { get; set; }

  public InputCmdMessage(GameTick tick, ushort subTick, KeyFunctionId inputFunctionId)
  {
    this.Tick = tick;
    this.SubTick = subTick;
    this.InputFunctionId = inputFunctionId;
  }

  public int CompareTo(InputCmdMessage? other)
  {
    if (other == null)
      return 1;
    if (this == other)
      return 0;
    return other == null ? 1 : this.InputSequence.CompareTo(other.InputSequence);
  }

  public override string ToString()
  {
    return $"tick={this.Tick}, subTick={this.SubTick}, seq={this.InputSequence} func={this.InputFunctionId}";
  }
}
