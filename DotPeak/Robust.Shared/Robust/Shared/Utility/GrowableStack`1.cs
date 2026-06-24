// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.GrowableStack`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Utility;

internal ref struct GrowableStack<T> where T : unmanaged
{
  private Span<T> _stack;
  private int _count;
  private int _capacity;

  public GrowableStack(Span<T> stackSpace)
  {
    this._stack = stackSpace;
    this._capacity = stackSpace.Length;
    this._count = 0;
  }

  internal ref T this[int index] => ref this._stack[index];

  public void Push(in T element)
  {
    if (this._count == this._capacity)
    {
      this._capacity *= 2;
      Span<T> stack = this._stack;
      this._stack = (Span<T>) GC.AllocateUninitializedArray<T>(this._capacity);
      stack.CopyTo(this._stack);
    }
    this._stack[this._count] = element;
    ++this._count;
  }

  public T Pop()
  {
    --this._count;
    return this._stack[this._count];
  }

  public int GetCount() => this._count;
}
