// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.UniqueIndexExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Utility;

public static class UniqueIndexExtensions
{
  public static void Clear<TKey, TValue>(ref this UniqueIndex<TKey, TValue> index) where TKey : notnull
  {
    index = new UniqueIndex<TKey, TValue>();
  }
}
