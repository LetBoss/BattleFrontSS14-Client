// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.GridLineEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;

#nullable disable
namespace Robust.Shared.Utility;

public struct GridLineEnumerator
{
  private int _x;
  private int _y;
  private int _i;
  private int _numerator;
  private readonly int _dx1;
  private readonly int _dy1;
  private readonly int _dx2;
  private readonly int _dy2;
  private readonly int _longest;
  private readonly int _shortest;

  public GridLineEnumerator(Vector2i start, Vector2i finish)
    : this(start.X, start.Y, finish.X, finish.Y)
  {
  }

  public GridLineEnumerator(int x, int y, int x2, int y2)
  {
    this._x = x;
    this._y = y;
    int num1 = x2 - x;
    int num2 = y2 - y;
    this._dx1 = Math.Sign(num1);
    this._dy1 = Math.Sign(num2);
    this._dx2 = Math.Sign(num1);
    this._dy2 = 0;
    this._longest = Math.Abs(num1);
    this._shortest = Math.Abs(num2);
    if (this._longest <= this._shortest)
    {
      int shortest = this._shortest;
      int longest = this._longest;
      this._longest = shortest;
      this._shortest = longest;
      this._dx2 = 0;
      this._dy2 = Math.Sign(num2);
    }
    this._numerator = this._longest / 2;
    this._i = -1;
  }

  public Vector2i Current => new Vector2i(this._x, this._y);

  public bool MoveNext()
  {
    if (this._i >= this._longest)
      return false;
    ++this._i;
    if (this._i == 0)
      return true;
    this._numerator += this._shortest;
    if (this._numerator >= this._longest)
    {
      this._numerator -= this._longest;
      this._x += this._dx1;
      this._y += this._dy1;
    }
    else
    {
      this._x += this._dx2;
      this._y += this._dy2;
    }
    return true;
  }
}
