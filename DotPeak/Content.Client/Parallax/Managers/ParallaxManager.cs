// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Managers.ParallaxManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Parallax.Data;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Content.Client.Parallax.Managers;

public sealed class ParallaxManager : IParallaxManager
{
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _configurationManager;
  [Dependency]
  private IDependencyCollection _deps;
  private ISawmill _sawmill = Logger.GetSawmill("parallax");
  private readonly Dictionary<string, ParallaxLayerPrepared[]> _parallaxesLQ = new Dictionary<string, ParallaxLayerPrepared[]>();
  private readonly Dictionary<string, ParallaxLayerPrepared[]> _parallaxesHQ = new Dictionary<string, ParallaxLayerPrepared[]>();
  private readonly Dictionary<string, CancellationTokenSource> _loadingParallaxes = new Dictionary<string, CancellationTokenSource>();

  public Vector2 ParallaxAnchor { get; set; }

  public bool IsLoaded(string name) => this._parallaxesLQ.ContainsKey(name);

  public ParallaxLayerPrepared[] GetParallaxLayers(string name)
  {
    ParallaxLayerPrepared[] parallaxLayerPreparedArray1;
    ParallaxLayerPrepared[] parallaxLayerPreparedArray2;
    return this._configurationManager.GetCVar<bool>(CCVars.ParallaxLowQuality) ? (this._parallaxesLQ.TryGetValue(name, out parallaxLayerPreparedArray1) ? parallaxLayerPreparedArray1 : Array.Empty<ParallaxLayerPrepared>()) : (this._parallaxesHQ.TryGetValue(name, out parallaxLayerPreparedArray2) ? parallaxLayerPreparedArray2 : Array.Empty<ParallaxLayerPrepared>());
  }

  public void UnloadParallax(string name)
  {
    CancellationTokenSource cancellationTokenSource;
    if (this._loadingParallaxes.TryGetValue(name, out cancellationTokenSource))
    {
      this._sawmill.Debug("Cancelling loading parallax " + name);
      cancellationTokenSource.Cancel();
      this._loadingParallaxes.Remove(name, out CancellationTokenSource _);
    }
    else
    {
      this._sawmill.Debug("Unloading parallax " + name);
      ParallaxLayerPrepared[] parallaxLayerPreparedArray;
      if (this._parallaxesLQ.Remove(name, out parallaxLayerPreparedArray))
      {
        foreach (ParallaxLayerPrepared parallaxLayerPrepared in parallaxLayerPreparedArray)
          parallaxLayerPrepared.Config.Texture.Unload(this._deps);
      }
      if (!this._parallaxesHQ.Remove(name, out parallaxLayerPreparedArray))
        return;
      foreach (ParallaxLayerPrepared parallaxLayerPrepared in parallaxLayerPreparedArray)
        parallaxLayerPrepared.Config.Texture.Unload(this._deps);
    }
  }

  public async void LoadDefaultParallax()
  {
    this._sawmill.Level = new LogLevel?((LogLevel) 2);
    await this.LoadParallaxByName("Default");
  }

  public async Task LoadParallaxByName(string name)
  {
    CancellationToken cancel;
    List<ParallaxLayerPrepared> loadedLayers;
    if (this._parallaxesLQ.ContainsKey(name))
    {
      cancel = new CancellationToken();
      loadedLayers = (List<ParallaxLayerPrepared>) null;
    }
    else if (this._loadingParallaxes.ContainsKey(name))
    {
      cancel = new CancellationToken();
      loadedLayers = (List<ParallaxLayerPrepared>) null;
    }
    else
    {
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      this._loadingParallaxes[name] = cancellationTokenSource;
      cancel = cancellationTokenSource.Token;
      this._sawmill.Debug("Loading parallax " + name);
      loadedLayers = new List<ParallaxLayerPrepared>();
      try
      {
        ParallaxPrototype parallaxPrototype = this._prototypeManager.Index<ParallaxPrototype>(name);
        ParallaxLayerPrepared[][] layers;
        if (parallaxPrototype.LayersLQUseHQ)
        {
          layers = new ParallaxLayerPrepared[2][];
          ParallaxLayerPrepared[][] parallaxLayerPreparedArray1 = layers;
          ParallaxLayerPrepared[][] parallaxLayerPreparedArray2 = layers;
          parallaxLayerPreparedArray1[0] = parallaxLayerPreparedArray2[1] = await this.LoadParallaxLayers(parallaxPrototype.Layers, loadedLayers, cancel);
          parallaxLayerPreparedArray1 = (ParallaxLayerPrepared[][]) null;
          parallaxLayerPreparedArray2 = (ParallaxLayerPrepared[][]) null;
        }
        else
          layers = await Task.WhenAll<ParallaxLayerPrepared[]>(this.LoadParallaxLayers(parallaxPrototype.Layers, loadedLayers, cancel), this.LoadParallaxLayers(parallaxPrototype.LayersLQ, loadedLayers, cancel));
        cancel.ThrowIfCancellationRequested();
        this._loadingParallaxes.Remove(name);
        this._parallaxesLQ[name] = layers[1];
        this._parallaxesHQ[name] = layers[0];
        this._sawmill.Verbose($"Loading parallax {name} completed");
        layers = (ParallaxLayerPrepared[][]) null;
        cancel = new CancellationToken();
        loadedLayers = (List<ParallaxLayerPrepared>) null;
      }
      catch (OperationCanceledException ex)
      {
        this._sawmill.Verbose($"Loading parallax {name} cancelled");
        using (List<ParallaxLayerPrepared>.Enumerator enumerator = loadedLayers.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Config.Texture.Unload(this._deps);
          cancel = new CancellationToken();
          loadedLayers = (List<ParallaxLayerPrepared>) null;
        }
      }
      catch (Exception ex)
      {
        this._sawmill.Error($"Failed to loaded parallax {name}: {ex}");
        cancel = new CancellationToken();
        loadedLayers = (List<ParallaxLayerPrepared>) null;
      }
    }
  }

  private async Task<ParallaxLayerPrepared[]> LoadParallaxLayers(
    List<ParallaxLayerConfig> layersIn,
    List<ParallaxLayerPrepared> loadedLayers,
    CancellationToken cancel = default (CancellationToken))
  {
    Task<ParallaxLayerPrepared>[] taskArray = new Task<ParallaxLayerPrepared>[layersIn.Count];
    for (int index = 0; index < layersIn.Count; ++index)
      taskArray[index] = this.LoadParallaxLayer(layersIn[index], loadedLayers, cancel);
    return await Task.WhenAll<ParallaxLayerPrepared>(taskArray);
  }

  private async Task<ParallaxLayerPrepared> LoadParallaxLayer(
    ParallaxLayerConfig config,
    List<ParallaxLayerPrepared> loadedLayers,
    CancellationToken cancel = default (CancellationToken))
  {
    ParallaxLayerPrepared parallaxLayerPrepared = new ParallaxLayerPrepared();
    parallaxLayerPrepared.Texture = await config.Texture.GenerateTexture(cancel);
    parallaxLayerPrepared.Config = config;
    ParallaxLayerPrepared parallaxLayerPrepared1 = parallaxLayerPrepared;
    parallaxLayerPrepared = new ParallaxLayerPrepared();
    loadedLayers.Add(parallaxLayerPrepared1);
    return parallaxLayerPrepared1;
  }
}
