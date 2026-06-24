// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.ModLoaderExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Reflection;

#nullable enable
namespace Robust.Shared.ContentPack;

public static class ModLoaderExt
{
  public static bool IsContentType(this IModLoader modLoader, Type type)
  {
    return modLoader.IsContentAssembly(type.Assembly);
  }

  public static bool IsContentTypeAccessAllowed(this IModLoader modLoader, Type type)
  {
    return modLoader.IsContentType(type) || type.GetCustomAttribute(typeof (ContentAccessAllowedAttribute)) != null;
  }
}
