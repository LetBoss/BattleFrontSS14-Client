// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.INetConfigurationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Network;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Configuration;

[NotContentImplementable]
public interface INetConfigurationManager : IConfigurationManager
{
  void SetupNetworking();

  List<(string name, object value)> GetReplicatedVars(bool all = false);

  void TickProcessMessages();

  void FlushMessages();

  T GetClientCVar<T>(INetChannel channel, string name);

  T GetClientCVar<T>(INetChannel channel, CVarDef<T> definition) where T : notnull
  {
    return this.GetClientCVar<T>(channel, definition.Name);
  }
}
