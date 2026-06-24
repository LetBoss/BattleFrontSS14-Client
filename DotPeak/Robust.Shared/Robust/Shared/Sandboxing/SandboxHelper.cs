// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Sandboxing.SandboxHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Robust.Shared.Sandboxing;

internal sealed class SandboxHelper : ISandboxHelper
{
  [Dependency]
  private readonly IModLoader _modLoader;

  public object CreateInstance(Type type)
  {
    return this._modLoader.IsContentTypeAccessAllowed(type) ? Activator.CreateInstance(type) : throw new SandboxArgumentException("Creating non-content types is not allowed.");
  }
}
