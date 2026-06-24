// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.FixedArray16`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

internal struct FixedArray16<T>
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

  public Span<T> AsSpan => MemoryMarshal.CreateSpan<T>(ref this._00, 16 /*0x10*/);
}
