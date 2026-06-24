// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.XamlExtensions.TexExtension
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Robust.Client.ResourceManagement;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.UserInterface.XamlExtensions;

public sealed class TexExtension
{
  private IResourceCache _resourceCache;

  public string Path { get; }

  public TexExtension(string path)
  {
    this._resourceCache = IoCManager.Resolve<IResourceCache>();
    this.Path = path;
  }

  public object ProvideValue() => (object) this._resourceCache.GetTexture(this.Path);
}
