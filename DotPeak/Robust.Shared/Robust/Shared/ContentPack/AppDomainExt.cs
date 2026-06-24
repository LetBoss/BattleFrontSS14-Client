// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.AppDomainExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using System;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Robust.Shared.ContentPack;

public static class AppDomainExt
{
  public static Assembly GetAssemblyByName(this AppDomain domain, string name)
  {
    ValueList<Assembly> source = new ValueList<Assembly>(1);
    foreach (Assembly assembly in domain.GetAssemblies())
    {
      if (!(assembly.GetName().Name != name))
        source.Add(assembly);
    }
    if (source.Count != 1)
    {
      string str = string.Join(" ", source.Select<Assembly, string>((Func<Assembly, string>) (o => o.GetName().Name)));
      throw new InvalidOperationException($"Expected 1 assembly for {name}, found {source.Count}. Found {str}");
    }
    return source[0];
  }
}
