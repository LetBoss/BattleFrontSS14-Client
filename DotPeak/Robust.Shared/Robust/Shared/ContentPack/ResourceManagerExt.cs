// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.ResourceManagerExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System.IO;

#nullable enable
namespace Robust.Shared.ContentPack;

public static class ResourceManagerExt
{
  public static Stream? ContentFileReadOrNull(this IResourceManager res, ResPath path)
  {
    Stream fileStream;
    return res.TryContentFileRead(new ResPath?(path), out fileStream) ? fileStream : (Stream) null;
  }
}
