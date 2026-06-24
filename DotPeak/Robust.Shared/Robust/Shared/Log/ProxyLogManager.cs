// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Log.ProxyLogManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Log;

public sealed class ProxyLogManager : ILogManager
{
  private readonly ILogManager _impl;

  public ProxyLogManager(ILogManager impl) => this._impl = impl;

  ISawmill ILogManager.RootSawmill => this._impl.RootSawmill;

  ISawmill ILogManager.GetSawmill(string name) => this._impl.GetSawmill(name);

  public IEnumerable<ISawmill> AllSawmills => this._impl.AllSawmills;
}
