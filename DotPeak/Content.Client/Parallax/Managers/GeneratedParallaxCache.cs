// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Managers.GeneratedParallaxCache
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Nett;
using Robust.Client.Graphics;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.ContentPack;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Parallax.Managers;

public sealed class GeneratedParallaxCache : IPostInjectInit
{
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IResourceManager _res;
  [Dependency]
  private ILogManager _logManager;
  private readonly Dictionary<string, GeneratedParallaxCache.CacheDatum> _data = new Dictionary<string, GeneratedParallaxCache.CacheDatum>();
  private ISawmill _sawmill;

  public Task<Texture> Load(string id, ResPath configPath, CancellationToken cancel = default (CancellationToken))
  {
    GeneratedParallaxCache.CacheDatum cacheDatum;
    if (!this._data.TryGetValue(id, out cacheDatum))
    {
      this._sawmill.Verbose($"Loading new generated layer {id} with config path {configPath}");
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      Task<Texture> task = this.LoadTask(id, configPath, cancellationTokenSource.Token);
      cacheDatum = new GeneratedParallaxCache.CacheDatum()
      {
        CancellationSource = cancellationTokenSource,
        ConfigPath = configPath,
        LoadTask = task
      };
      this._data.Add(id, cacheDatum);
    }
    else if (ResPath.op_Inequality(cacheDatum.ConfigPath, configPath))
      throw new InvalidOperationException("Generated parallax layers with the same ID must have the same config path!");
    ++cacheDatum.RefCount;
    if (!cacheDatum.LoadTask.IsCompleted)
      cancel.Register((Action) (() => this.Unload(id)));
    return cacheDatum.LoadTask;
  }

  public void Unload(string id)
  {
    GeneratedParallaxCache.CacheDatum cacheDatum;
    if (!this._data.TryGetValue(id, out cacheDatum))
      throw new InvalidOperationException("Layer is not cached!");
    --cacheDatum.RefCount;
    if (cacheDatum.RefCount != 0)
      return;
    this._sawmill.Verbose("Unloading generated layer " + id);
    cacheDatum.CancellationSource.Cancel();
    this._data.Remove(id);
  }

  private async Task<Texture> LoadTask(string id, ResPath configPath, CancellationToken cancel)
  {
    return await this.GenerateTexture(id, configPath, cancel);
  }

  private async Task<Texture> GenerateTexture(
    string id,
    ResPath configPath,
    CancellationToken cancel)
  {
    string parallaxConfig = this.GetParallaxConfig(configPath);
    if (parallaxConfig == null)
    {
      this._sawmill.Error($"Parallax config not found or unreadable: {configPath}");
      return Texture.Transparent;
    }
    bool cvar = this._cfg.GetCVar<bool>(CCVars.ParallaxDebug);
    string str;
    if (cvar || !WritableDirProviderExt.TryReadAllText(this._res.UserData, GeneratedParallaxCache.PreviousConfigPath(id), ref str) || str != parallaxConfig)
    {
      TomlTable config = Toml.ReadString(parallaxConfig);
      await this.UpdateCachedTexture(id, config, cvar, cancel);
      using (StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(this._res.UserData, GeneratedParallaxCache.PreviousConfigPath(id)))
        streamWriter.Write(parallaxConfig);
    }
    try
    {
      return this.GetCachedTexture(id);
    }
    catch (Exception ex1)
    {
      this._sawmill.Error($"Couldn't retrieve parallax cached texture: {ex1}");
      try
      {
        this._res.UserData.Delete(GeneratedParallaxCache.PreviousConfigPath(id));
      }
      catch (Exception ex2)
      {
      }
      return Texture.Transparent;
    }
  }

  private async Task UpdateCachedTexture(
    string id,
    TomlTable config,
    bool saveDebugLayers,
    CancellationToken cancel)
  {
    List<Image<Rgba32>> debugImages = saveDebugLayers ? new List<Image<Rgba32>>() : (List<Image<Rgba32>>) null;
    Stream imageStream;
    using (Image<Rgba32> newParallexImage = await Task.Run<Image<Rgba32>>((Func<Image<Rgba32>>) (() => ParallaxGenerator.GenerateParallax(config, new Size(1920, 1080), this._sawmill, debugImages, cancel)), cancel))
    {
      cancel.ThrowIfCancellationRequested();
      imageStream = WritableDirProviderExt.OpenWrite(this._res.UserData, GeneratedParallaxCache.CachedImagePath(id));
      object obj = (object) null;
      int num = 0;
      try
      {
        await ImageExtensions.SaveAsPngAsync((Image) newParallexImage, imageStream, cancel);
        if (saveDebugLayers)
        {
          for (int i = 0; i < debugImages.Count; ++i)
          {
            await using (Stream debugImageStream = WritableDirProviderExt.OpenWrite(this._res.UserData, new ResPath($"/parallax_{id}debug_{i}.png")))
              await ImageExtensions.SaveAsPngAsync((Image) debugImages[i], debugImageStream, cancel);
          }
        }
        num = 1;
      }
      catch (object ex)
      {
        obj = ex;
      }
      if (imageStream != null)
        await imageStream.DisposeAsync();
      object obj1 = obj;
      if (obj1 != null)
      {
        if (!(obj1 is Exception source))
          throw obj1;
        ExceptionDispatchInfo.Capture(source).Throw();
      }
      if (num != 1)
        obj = (object) null;
      else
        goto label_32;
    }
    imageStream = (Stream) null;
    throw null;
label_32:;
  }

  private Texture GetCachedTexture(string id)
  {
    using (Stream stream = WritableDirProviderExt.OpenRead(this._res.UserData, GeneratedParallaxCache.CachedImagePath(id)))
      return Texture.LoadFromPNGStream(stream, "Parallax " + id, new TextureLoadParameters?());
  }

  private string? GetParallaxConfig(ResPath configPath)
  {
    Stream stream;
    if (!this._res.TryContentFileRead(new ResPath?(configPath), ref stream))
      return (string) null;
    using (StreamReader streamReader = new StreamReader(stream, EncodingHelpers.UTF8))
      return streamReader.ReadToEnd().Replace(Environment.NewLine, "\n");
  }

  private static ResPath CachedImagePath(string identifier)
  {
    return new ResPath($"/parallax_{identifier}cache.png");
  }

  private static ResPath PreviousConfigPath(string identifier)
  {
    return new ResPath($"/parallax_{identifier}config_old");
  }

  void IPostInjectInit.PostInject()
  {
    this._sawmill = this._logManager.GetSawmill("parallax.generated");
  }

  private sealed class CacheDatum
  {
    public required ResPath ConfigPath;
    public required Task<Texture> LoadTask;
    public required CancellationTokenSource CancellationSource;
    public ValueList<CancellationTokenRegistration> CancelRegistrations;
    public int RefCount;
  }
}
