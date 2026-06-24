// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.IConfigurationManagerInternal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Configuration;

internal interface IConfigurationManagerInternal : IConfigurationManager
{
  void OverrideConVars(IEnumerable<(string key, string value)> cVars);

  void LoadCVarsFromAssembly(Assembly assembly);

  void LoadCVarsFromType(Type containingType);

  void SetVirtualConfig();

  void Initialize(bool isServer);

  void Shutdown();

  HashSet<string> LoadFromFile(string configFile);

  void SetSaveFile(string configFile);

  void CheckUnusedCVars();
}
