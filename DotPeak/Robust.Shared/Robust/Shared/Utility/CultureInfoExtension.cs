// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.CultureInfoExtension
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Globalization;

#nullable enable
namespace Robust.Shared.Utility;

public static class CultureInfoExtension
{
  public static bool NameEquals(this CultureInfo cultureInfo, CultureInfo otherCultureInfo)
  {
    return cultureInfo.Name == otherCultureInfo.Name;
  }
}
