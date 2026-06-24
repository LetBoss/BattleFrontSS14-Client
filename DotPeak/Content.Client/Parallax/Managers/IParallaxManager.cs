// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Managers.IParallaxManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Numerics;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Parallax.Managers;

public interface IParallaxManager
{
  Vector2 ParallaxAnchor { get; set; }

  bool IsLoaded(string name);

  ParallaxLayerPrepared[] GetParallaxLayers(string name);

  void LoadDefaultParallax();

  Task LoadParallaxByName(string name);

  void UnloadParallax(string name);
}
