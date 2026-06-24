// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.IModLoaderInternal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace Robust.Shared.ContentPack;

internal interface IModLoaderInternal : IModLoader
{
  bool TryLoadModulesFrom(ResPath mountPath, string filterPrefix);

  bool TryLoadModules(IEnumerable<ResPath> paths);

  void LoadGameAssembly(Stream assembly, Stream? symbols = null, bool skipVerify = false);

  void LoadGameAssembly(string diskPath, bool skipVerify = false);

  void BroadcastRunLevel(ModRunLevel level);

  void BroadcastUpdate(ModUpdateLevel level, FrameEventArgs frameEventArgs);

  bool TryLoadAssembly(string assemblyName);

  void SetUseLoadContext(bool useLoadContext);

  void SetEnableSandboxing(bool sandboxing);

  Func<string, Stream?>? VerifierExtraLoadHandler { get; set; }

  void AddEngineModuleDirectory(string dir);

  void Shutdown();

  event ExtraModuleLoad ExtraModuleLoaders;
}
