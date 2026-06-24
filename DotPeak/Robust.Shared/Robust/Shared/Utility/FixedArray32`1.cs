// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.FixedArray32`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

internal struct FixedArray32<T>
{
  public T _00;
  public T _01;
  public T _02;
  public T _03;
  public T _04;
  public T _05;
  public T _06;
  public T _07;
  public T _08;
  public T _09;
  public T _10;
  public T _11;
  public T _12;
  public T _13;
  public T _14;
  public T _15;
  public T _16;
  public T _17;
  public T _18;
  public T _19;
  public T _20;
  public T _21;
  public T _22;
  public T _23;
  public T _24;
  public T _25;
  public T _26;
  public T _27;
  public T _28;
  public T _29;
  public T _30;
  public T _31;

  public Span<T> AsSpan => MemoryMarshal.CreateSpan<T>(ref this._00, 32 /*0x20*/);
}
