using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

namespace Content.Client.Parallax.Managers;

public sealed class GeneratedParallaxCache : IPostInjectInit
{
	private sealed class CacheDatum
	{
		public required ResPath ConfigPath;

		public required Task<Texture> LoadTask;

		public required CancellationTokenSource CancellationSource;

		public ValueList<CancellationTokenRegistration> CancelRegistrations;

		public int RefCount;
	}

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IResourceManager _res;

	[Dependency]
	private ILogManager _logManager;

	private readonly Dictionary<string, CacheDatum> _data = new Dictionary<string, CacheDatum>();

	private ISawmill _sawmill;

	public Task<Texture> Load(string id, ResPath configPath, CancellationToken cancel = default(CancellationToken))
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (!_data.TryGetValue(id, out CacheDatum value))
		{
			_sawmill.Verbose($"Loading new generated layer {id} with config path {configPath}");
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			Task<Texture> loadTask = LoadTask(id, configPath, cancellationTokenSource.Token);
			value = new CacheDatum
			{
				CancellationSource = cancellationTokenSource,
				ConfigPath = configPath,
				LoadTask = loadTask
			};
			_data.Add(id, value);
		}
		else if (value.ConfigPath != configPath)
		{
			throw new InvalidOperationException("Generated parallax layers with the same ID must have the same config path!");
		}
		value.RefCount++;
		if (!value.LoadTask.IsCompleted)
		{
			cancel.Register(delegate
			{
				Unload(id);
			});
		}
		return value.LoadTask;
	}

	public void Unload(string id)
	{
		if (!_data.TryGetValue(id, out CacheDatum value))
		{
			throw new InvalidOperationException("Layer is not cached!");
		}
		value.RefCount--;
		if (value.RefCount == 0)
		{
			_sawmill.Verbose("Unloading generated layer " + id);
			value.CancellationSource.Cancel();
			_data.Remove(id);
		}
	}

	private async Task<Texture> LoadTask(string id, ResPath configPath, CancellationToken cancel)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return await GenerateTexture(id, configPath, cancel);
	}

	private async Task<Texture> GenerateTexture(string id, ResPath configPath, CancellationToken cancel)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		string parallaxConfig = GetParallaxConfig(configPath);
		if (parallaxConfig == null)
		{
			_sawmill.Error($"Parallax config not found or unreadable: {configPath}");
			return Texture.Transparent;
		}
		bool cVar = _cfg.GetCVar<bool>(CCVars.ParallaxDebug);
		string text = default(string);
		if (cVar || !WritableDirProviderExt.TryReadAllText(_res.UserData, PreviousConfigPath(id), ref text) || text != parallaxConfig)
		{
			TomlTable config = Toml.ReadString(parallaxConfig);
			await UpdateCachedTexture(id, config, cVar, cancel);
			using StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(_res.UserData, PreviousConfigPath(id));
			streamWriter.Write(parallaxConfig);
		}
		try
		{
			return GetCachedTexture(id);
		}
		catch (Exception value)
		{
			_sawmill.Error($"Couldn't retrieve parallax cached texture: {value}");
			try
			{
				_res.UserData.Delete(PreviousConfigPath(id));
			}
			catch (Exception)
			{
			}
			return Texture.Transparent;
		}
	}

	private async Task UpdateCachedTexture(string id, TomlTable config, bool saveDebugLayers, CancellationToken cancel)
	{
		List<Image<Rgba32>> debugImages = (saveDebugLayers ? new List<Image<Rgba32>>() : null);
		Image<Rgba32> newParallexImage = await Task.Run(() => ParallaxGenerator.GenerateParallax(config, new Size(1920, 1080), _sawmill, debugImages, cancel), cancel);
		try
		{
			cancel.ThrowIfCancellationRequested();
			await using Stream imageStream = WritableDirProviderExt.OpenWrite(_res.UserData, CachedImagePath(id));
			await ImageExtensions.SaveAsPngAsync((Image)(object)newParallexImage, imageStream, cancel);
			if (!saveDebugLayers)
			{
				return;
			}
			for (int i = 0; i < debugImages.Count; i++)
			{
				Image<Rgba32> val = debugImages[i];
				await using Stream debugImageStream = WritableDirProviderExt.OpenWrite(_res.UserData, new ResPath($"/parallax_{id}debug_{i}.png"));
				await ImageExtensions.SaveAsPngAsync((Image)(object)val, debugImageStream, cancel);
			}
		}
		finally
		{
			((IDisposable)newParallexImage)?.Dispose();
		}
	}

	private Texture GetCachedTexture(string id)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		using Stream stream = WritableDirProviderExt.OpenRead(_res.UserData, CachedImagePath(id));
		return Texture.LoadFromPNGStream(stream, "Parallax " + id, (TextureLoadParameters?)null);
	}

	private string? GetParallaxConfig(ResPath configPath)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Stream stream = default(Stream);
		if (!_res.TryContentFileRead((ResPath?)configPath, ref stream))
		{
			return null;
		}
		using StreamReader streamReader = new StreamReader(stream, EncodingHelpers.UTF8);
		return streamReader.ReadToEnd().Replace(Environment.NewLine, "\n");
	}

	private static ResPath CachedImagePath(string identifier)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new ResPath("/parallax_" + identifier + "cache.png");
	}

	private static ResPath PreviousConfigPath(string identifier)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new ResPath("/parallax_" + identifier + "config_old");
	}

	void IPostInjectInit.PostInject()
	{
		_sawmill = _logManager.GetSawmill("parallax.generated");
	}
}
