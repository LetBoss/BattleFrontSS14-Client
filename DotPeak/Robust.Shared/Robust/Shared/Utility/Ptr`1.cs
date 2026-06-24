// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.Ptr`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Utility;

internal struct Ptr<T> where T : unmanaged
{
  public unsafe T* P;

  public static unsafe implicit operator T*(Ptr<T> t) => t.P;

  public static unsafe implicit operator Ptr<T>(T* ptr)
  {
    return new Ptr<T>() { P = ptr };
  }
}
