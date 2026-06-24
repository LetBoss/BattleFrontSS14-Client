// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.LineEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Utility;

internal struct LineEnumerator(ReadOnlyMemory<char> text)
{
  private readonly ReadOnlyMemory<char> _text = text;
  private int _curPos = 0;

  public bool MoveNext(out int start, out int end)
  {
    if (this._curPos == this._text.Length)
    {
      start = 0;
      end = 0;
      return false;
    }
    ReadOnlySpan<char> span = this._text.Span;
    ref ReadOnlySpan<char> local = ref span;
    int curPos = this._curPos;
    int num1 = local.Slice(curPos, local.Length - curPos).IndexOf<char>('\n');
    int num2 = num1 != -1 ? num1 + this._curPos + 1 : this._text.Length;
    start = this._curPos;
    end = num2;
    this._curPos = num2;
    return true;
  }
}
