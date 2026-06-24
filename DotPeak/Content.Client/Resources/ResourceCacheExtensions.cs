// Decompiled with JetBrains decompiler
// Type: Content.Client.Resources.ResourceCacheExtensions
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.Resources;

public static class ResourceCacheExtensions
{
  public static Texture GetTexture(this IResourceCache cache, ResPath path)
  {
    return TextureResource.op_Implicit(cache.GetResource<TextureResource>(path, true));
  }

  public static Texture GetTexture(this IResourceCache cache, string path)
  {
    return cache.GetTexture(new ResPath(path));
  }

  public static Font GetFont(this IResourceCache cache, ResPath path, int size)
  {
    return (Font) new VectorFont(cache.GetResource<FontResource>(path, true), size);
  }

  public static Font GetFont(this IResourceCache cache, string path, int size)
  {
    return cache.GetFont(new ResPath(path), size);
  }

  public static Font GetFont(this IResourceCache cache, ResPath[] path, int size)
  {
    Font[] fontArray = new Font[path.Length];
    for (int index = 0; index < path.Length; ++index)
      fontArray[index] = (Font) new VectorFont(cache.GetResource<FontResource>(path[index], true), size);
    return (Font) new StackedFont(fontArray);
  }

  public static Font GetFont(this IResourceCache cache, string[] path, int size)
  {
    ResPath[] path1 = new ResPath[path.Length];
    for (int index = 0; index < path.Length; ++index)
      path1[index] = new ResPath(path[index]);
    return cache.GetFont(path1, size);
  }
}
