// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.StringEnumerateHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace Robust.Shared.Utility;

internal static class StringEnumerateHelpers
{
  internal struct SubstringRuneEnumerator(string source, int firstChar) : 
    IEnumerable<Rune>,
    IEnumerable,
    IEnumerator<Rune>,
    IEnumerator,
    IDisposable
  {
    private readonly string _source = source;
    private int _nextChar = firstChar;
    private Rune _current = new Rune();

    public bool MoveNext()
    {
      if (this._nextChar >= this._source.Length)
        return false;
      if (!Rune.TryGetRuneAt(this._source, this._nextChar, out this._current))
        this._current = Rune.ReplacementChar;
      this._nextChar += this._current.Utf16SequenceLength;
      return true;
    }

    public void Reset() => throw new NotSupportedException();

    public readonly Rune Current => this._current;

    object IEnumerator.Current => (object) this.Current;

    public void Dispose()
    {
    }

    public StringEnumerateHelpers.SubstringRuneEnumerator GetEnumerator() => this;

    IEnumerator<Rune> IEnumerable<Rune>.GetEnumerator() => (IEnumerator<Rune>) this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }

  internal struct SubstringReverseRuneEnumerator(string source, int startChar) : 
    IEnumerator<Rune>,
    IEnumerator,
    IDisposable,
    IEnumerable<Rune>,
    IEnumerable
  {
    private string _source = source;
    private int _nextChar = startChar - 1;
    private Rune _current = new Rune();

    public bool MoveNext()
    {
      if (this._nextChar < 0)
        return false;
      char ch1 = this._source[this._nextChar];
      if (!char.IsSurrogate(ch1))
        this._current = new Rune(ch1);
      else if (char.IsLowSurrogate(ch1) && this._nextChar >= 1)
      {
        char ch2 = this._source[this._nextChar - 1];
        this._current = !char.IsHighSurrogate(ch2) ? Rune.ReplacementChar : new Rune(ch2, ch1);
      }
      else
        this._current = Rune.ReplacementChar;
      this._nextChar -= this._current.Utf16SequenceLength;
      return true;
    }

    public void Reset() => throw new NotSupportedException();

    public Rune Current => this._current;

    object IEnumerator.Current => (object) this.Current;

    public void Dispose()
    {
    }

    public StringEnumerateHelpers.SubstringReverseRuneEnumerator GetEnumerator() => this;

    IEnumerator<Rune> IEnumerable<Rune>.GetEnumerator() => (IEnumerator<Rune>) this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
