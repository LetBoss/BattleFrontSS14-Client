// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.IResourceManagerInternal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

#nullable enable
namespace Robust.Shared.ContentPack;

internal interface IResourceManagerInternal : IResourceManager
{
  void Initialize(string? userData, bool hideUserDataDir);

  void MountStreamAt(MemoryStream stream, ResPath path);

  void MountDefaultContentPack();

  void MountContentPack(string pack, ResPath? prefix = null);

  void MountContentPack(Stream zipStream, ResPath? prefix = null);

  void MountContentDirectory(string path, ResPath? prefix = null);

  bool TryGetDiskFilePath(ResPath path, [NotNullWhen(true)] out string? diskPath);

  IEnumerable<string> GetContentRoots();
}
