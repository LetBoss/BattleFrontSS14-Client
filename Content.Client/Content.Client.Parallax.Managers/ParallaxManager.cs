using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Content.Client.Parallax.Data;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;

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

	public bool IsLoaded(string name)
	{
		return _parallaxesLQ.ContainsKey(name);
	}

	public ParallaxLayerPrepared[] GetParallaxLayers(string name)
	{
		if (_configurationManager.GetCVar<bool>(CCVars.ParallaxLowQuality))
		{
			if (_parallaxesLQ.TryGetValue(name, out ParallaxLayerPrepared[] value))
			{
				return value;
			}
			return Array.Empty<ParallaxLayerPrepared>();
		}
		if (_parallaxesHQ.TryGetValue(name, out ParallaxLayerPrepared[] value2))
		{
			return value2;
		}
		return Array.Empty<ParallaxLayerPrepared>();
	}

	public void UnloadParallax(string name)
	{
		if (_loadingParallaxes.TryGetValue(name, out CancellationTokenSource value))
		{
			_sawmill.Debug("Cancelling loading parallax " + name);
			value.Cancel();
			_loadingParallaxes.Remove(name, out CancellationTokenSource _);
			return;
		}
		_sawmill.Debug("Unloading parallax " + name);
		if (_parallaxesLQ.Remove(name, out ParallaxLayerPrepared[] value3))
		{
			ParallaxLayerPrepared[] array = value3;
			foreach (ParallaxLayerPrepared parallaxLayerPrepared in array)
			{
				parallaxLayerPrepared.Config.Texture.Unload(_deps);
			}
		}
		if (_parallaxesHQ.Remove(name, out value3))
		{
			ParallaxLayerPrepared[] array = value3;
			foreach (ParallaxLayerPrepared parallaxLayerPrepared2 in array)
			{
				parallaxLayerPrepared2.Config.Texture.Unload(_deps);
			}
		}
	}

	public async void LoadDefaultParallax()
	{
		_sawmill.Level = (LogLevel)2;
		await LoadParallaxByName("Default");
	}

	public async Task LoadParallaxByName(string name)
	{
		if (_parallaxesLQ.ContainsKey(name) || _loadingParallaxes.ContainsKey(name))
		{
			return;
		}
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		_loadingParallaxes[name] = cancellationTokenSource;
		CancellationToken cancel = cancellationTokenSource.Token;
		_sawmill.Debug("Loading parallax " + name);
		List<ParallaxLayerPrepared> loadedLayers = new List<ParallaxLayerPrepared>();
		try
		{
			ParallaxPrototype parallaxPrototype = _prototypeManager.Index<ParallaxPrototype>(name);
			ParallaxLayerPrepared[][] layers;
			if (parallaxPrototype.LayersLQUseHQ)
			{
				layers = new ParallaxLayerPrepared[2][];
				ParallaxLayerPrepared[][] array = layers;
				ParallaxLayerPrepared[][] array2 = layers;
				array[0] = (array2[1] = await LoadParallaxLayers(parallaxPrototype.Layers, loadedLayers, cancel));
			}
			else
			{
				layers = await Task.WhenAll(new Task<ParallaxLayerPrepared[]>[2]
				{
					LoadParallaxLayers(parallaxPrototype.Layers, loadedLayers, cancel),
					LoadParallaxLayers(parallaxPrototype.LayersLQ, loadedLayers, cancel)
				});
			}
			cancel.ThrowIfCancellationRequested();
			_loadingParallaxes.Remove(name);
			_parallaxesLQ[name] = layers[1];
			_parallaxesHQ[name] = layers[0];
			_sawmill.Verbose("Loading parallax " + name + " completed");
		}
		catch (OperationCanceledException)
		{
			_sawmill.Verbose("Loading parallax " + name + " cancelled");
			foreach (ParallaxLayerPrepared item in loadedLayers)
			{
				item.Config.Texture.Unload(_deps);
			}
		}
		catch (Exception value)
		{
			_sawmill.Error($"Failed to loaded parallax {name}: {value}");
		}
	}

	private async Task<ParallaxLayerPrepared[]> LoadParallaxLayers(List<ParallaxLayerConfig> layersIn, List<ParallaxLayerPrepared> loadedLayers, CancellationToken cancel = default(CancellationToken))
	{
		Task<ParallaxLayerPrepared>[] array = new Task<ParallaxLayerPrepared>[layersIn.Count];
		for (int i = 0; i < layersIn.Count; i++)
		{
			array[i] = LoadParallaxLayer(layersIn[i], loadedLayers, cancel);
		}
		return await Task.WhenAll(array);
	}

	private async Task<ParallaxLayerPrepared> LoadParallaxLayer(ParallaxLayerConfig config, List<ParallaxLayerPrepared> loadedLayers, CancellationToken cancel = default(CancellationToken))
	{
		ParallaxLayerPrepared parallaxLayerPrepared = new ParallaxLayerPrepared
		{
			Texture = await config.Texture.GenerateTexture(cancel),
			Config = config
		};
		loadedLayers.Add(parallaxLayerPrepared);
		return parallaxLayerPrepared;
	}
}
