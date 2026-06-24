// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.CommandBuffer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Console;

public sealed class CommandBuffer
{
  private const string DelayMarker = "-DELAY-";
  private int _tickrate;
  private int _delay;
  private readonly LinkedList<string> _commandBuffer = new LinkedList<string>();

  public void Append(string command) => this._commandBuffer.AddLast(command);

  public void Insert(string command) => this._commandBuffer.AddFirst(command);

  public void Tick(ushort tickRate)
  {
    this._tickrate = (int) tickRate;
    if (this._delay <= 0)
      return;
    --this._delay;
  }

  public bool TryGetCommand([MaybeNullWhen(false)] out string command)
  {
    LinkedListNode<string> first = this._commandBuffer.First;
    if (first == null)
    {
      command = (string) null;
      return false;
    }
    if (first.Value.Equals("-DELAY-"))
    {
      if (this._delay == 0)
      {
        this._commandBuffer.RemoveFirst();
        return this.TryGetCommand(out command);
      }
      command = (string) null;
      return false;
    }
    if (first.Value.StartsWith("wait "))
    {
      string s = first.Value.Substring(5);
      this._commandBuffer.RemoveFirst();
      int result;
      if (string.IsNullOrWhiteSpace(s) || !int.TryParse(s, out result))
        return this.TryGetCommand(out command);
      this._commandBuffer.AddFirst("-DELAY-");
      this._delay = result;
      command = (string) null;
      return false;
    }
    this._commandBuffer.RemoveFirst();
    command = first.Value;
    return true;
  }
}
