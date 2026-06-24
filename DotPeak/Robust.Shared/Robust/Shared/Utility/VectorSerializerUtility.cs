// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.VectorSerializerUtility
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Utility;

public sealed class VectorSerializerUtility
{
  private static char[] _separators = new char[2]
  {
    ',',
    'x'
  };

  public static bool TryParseArgs(string value, int count, [NotNullWhen(true)] out string[]? args)
  {
    foreach (char separator in VectorSerializerUtility._separators)
    {
      args = value.Split(separator);
      if (args.Length == count)
        return true;
    }
    args = (string[]) null;
    return false;
  }
}
