using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.TacticalMap;

public sealed class TacticalMapSettingsManager
{
	private const string SettingsPath = "/rmc_tactical_map_settings.yml";

	[Dependency]
	private IResourceManager _resourceMan;

	[Dependency]
	private ISerializationManager _serialization;

	private readonly HashSet<string> _modifiedSettings = new HashSet<string>();

	private readonly List<TacticalMapSettingRegistration> _defaultRegistrations = new List<TacticalMapSettingRegistration>();

	private readonly Dictionary<string, TacticalMapSettingRegistration> _currentSettings = new Dictionary<string, TacticalMapSettingRegistration>();

	private ISawmill _logger;

	public TacticalMapSettingsManager()
	{
		IoCManager.InjectDependencies<TacticalMapSettingsManager>(this);
		_logger = Logger.GetSawmill("tactical_map_settings");
		Initialize();
	}

	private void Initialize()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		RegisterDefaultSettings();
		ResPath val = default(ResPath);
		((ResPath)(ref val))._002Ector("/rmc_tactical_map_settings.yml");
		if (_resourceMan.UserData.Exists(val))
		{
			try
			{
				LoadSettingsFile(val, defaultRegistration: false, userData: true);
			}
			catch (Exception ex)
			{
				_logger.Error("Failed to load user tactical map settings: " + ex);
			}
		}
		if (_resourceMan.ContentFileExists(val))
		{
			LoadSettingsFile(val, defaultRegistration: true);
		}
	}

	private void RegisterDefaultSettings()
	{
		TacticalMapSettingRegistration[] array = new TacticalMapSettingRegistration[12]
		{
			new TacticalMapSettingRegistration
			{
				Key = "ZoomFactor",
				Value = 1f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "PanOffsetX",
				Value = 0f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "PanOffsetY",
				Value = 0f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "BlipSizeMultiplier",
				Value = 1f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "LineThickness",
				Value = 2f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "SelectedColorIndex",
				Value = 0,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "SettingsVisible",
				Value = false,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "LabelMode",
				Value = 2,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "WindowWidth",
				Value = 1000f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "WindowHeight",
				Value = 800f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "WindowPositionX",
				Value = -1f,
				PlanetId = null
			},
			new TacticalMapSettingRegistration
			{
				Key = "WindowPositionY",
				Value = -1f,
				PlanetId = null
			}
		};
		for (int i = 0; i < array.Length; i++)
		{
			TacticalMapSettingRegistration tacticalMapSettingRegistration = array[i];
			_defaultRegistrations.Add(tacticalMapSettingRegistration);
			if (!_modifiedSettings.Contains(GetSettingKey(tacticalMapSettingRegistration.Key, tacticalMapSettingRegistration.PlanetId)))
			{
				_currentSettings[GetSettingKey(tacticalMapSettingRegistration.Key, tacticalMapSettingRegistration.PlanetId)] = tacticalMapSettingRegistration;
			}
		}
	}

	private void LoadSettingsFile(ResPath file, bool defaultRegistration, bool userData = false)
	{
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		TextReader textReader = ((!userData) ? _resourceMan.ContentFileReadText(file) : WritableDirProviderExt.OpenText(_resourceMan.UserData, file));
		using (textReader)
		{
			try
			{
				MappingDataNode val = (MappingDataNode)DataNodeParser.ParseYamlStream(textReader).First().Root;
				DataNode val2 = default(DataNode);
				if (!val.TryGet("settings", ref val2))
				{
					return;
				}
				if (val2 == null)
				{
					_logger.Warning("Settings node is null, skipping settings load");
					return;
				}
				TacticalMapSettingRegistration[] array = null;
				try
				{
					array = _serialization.Read<TacticalMapSettingRegistration[]>(val2, (ISerializationContext)null, false, (InstantiationDelegate<TacticalMapSettingRegistration[]>)null, false);
				}
				catch (Exception value)
				{
					_logger.Error($"Failed to parse settings array: {value}");
					return;
				}
				if (array == null)
				{
					_logger.Warning("Parsed settings array is null, skipping settings load");
					return;
				}
				TacticalMapSettingRegistration[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					TacticalMapSettingRegistration tacticalMapSettingRegistration = array2[i];
					if (tacticalMapSettingRegistration.Key == null || string.IsNullOrEmpty(tacticalMapSettingRegistration.Key))
					{
						_logger.Warning("Skipping setting with null/empty key");
						continue;
					}
					if (tacticalMapSettingRegistration.Value == null)
					{
						_logger.Warning("Skipping setting '" + tacticalMapSettingRegistration.Key + "' with null value");
						continue;
					}
					string settingKey = GetSettingKey(tacticalMapSettingRegistration.Key, tacticalMapSettingRegistration.PlanetId);
					if (defaultRegistration)
					{
						_defaultRegistrations.Add(tacticalMapSettingRegistration);
						if (_modifiedSettings.Contains(settingKey))
						{
							continue;
						}
					}
					_currentSettings[settingKey] = tacticalMapSettingRegistration;
					if (!defaultRegistration)
					{
						_modifiedSettings.Add(settingKey);
					}
				}
				DataNode val3 = default(DataNode);
				if (defaultRegistration || !val.TryGet("unsetSettings", ref val3) || val3 == null)
				{
					return;
				}
				try
				{
					string[] array3 = _serialization.Read<string[]>(val3, (ISerializationContext)null, false, (InstantiationDelegate<string[]>)null, false);
					if (array3 == null)
					{
						return;
					}
					string[] array4 = array3;
					foreach (string text in array4)
					{
						if (!string.IsNullOrEmpty(text))
						{
							_modifiedSettings.Add(text);
							_currentSettings.Remove(text);
						}
					}
				}
				catch (Exception value2)
				{
					_logger.Error($"Failed to parse unsetSettings array: {value2}");
				}
			}
			catch (Exception value3)
			{
				_logger.Error($"Failed to parse settings file {file}: {value3}");
				if (!defaultRegistration)
				{
					_logger.Info("Continuing with default settings due to parse failure");
				}
			}
		}
	}

	private string GetSettingKey(string key, string? planetId)
	{
		if (!string.IsNullOrEmpty(planetId))
		{
			return key + "_" + planetId;
		}
		return key;
	}

	public void SaveToUserData()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		ResPath val = default(ResPath);
		((ResPath)(ref val))._002Ector("/rmc_tactical_map_settings.yml");
		try
		{
			using StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(_resourceMan.UserData, val);
			streamWriter.WriteLine("version: \"1\"");
			streamWriter.WriteLine("settings:");
			foreach (TacticalMapSettingRegistration item in from key in _modifiedSettings
				where _currentSettings.ContainsKey(key)
				select _currentSettings[key] into setting
				where setting.Key != null
				select setting)
			{
				streamWriter.WriteLine("  - Key: \"" + item.Key + "\"");
				streamWriter.WriteLine("    Value: " + FormatValueForYaml(item.Value));
				if (!string.IsNullOrEmpty(item.PlanetId))
				{
					streamWriter.WriteLine("    PlanetId: \"" + item.PlanetId + "\"");
				}
				else
				{
					streamWriter.WriteLine("    PlanetId: null");
				}
			}
			streamWriter.WriteLine("unsetSettings:");
			foreach (string item2 in _modifiedSettings.Where((string key) => !_currentSettings.ContainsKey(key)))
			{
				streamWriter.WriteLine("  - \"" + item2 + "\"");
			}
		}
		catch (Exception value)
		{
			_logger.Error($"Failed to save tactical map settings: {value}");
		}
	}

	private string FormatValueForYaml(object? value)
	{
		if (value == null)
		{
			return "null";
		}
		if (!(value is string text))
		{
			if (!(value is bool flag))
			{
				if (!(value is float num))
				{
					if (value is int num2)
					{
						return num2.ToString();
					}
					return "\"" + value.ToString() + "\"";
				}
				return num.ToString("F6");
			}
			return flag.ToString().ToLower();
		}
		return "\"" + text.Replace("\"", "\\\"") + "\"";
	}

	public TacticalMapSettings LoadSettings(string? planetId = null)
	{
		TacticalMapSettings tacticalMapSettings = new TacticalMapSettings();
		tacticalMapSettings.ZoomFactor = GetSettingValue("ZoomFactor", planetId, 1f);
		tacticalMapSettings.PanOffset = new Vector2(GetSettingValue("PanOffsetX", planetId, 0f), GetSettingValue("PanOffsetY", planetId, 0f));
		tacticalMapSettings.BlipSizeMultiplier = GetSettingValue("BlipSizeMultiplier", planetId, 1f);
		tacticalMapSettings.LineThickness = GetSettingValue("LineThickness", planetId, 2f);
		tacticalMapSettings.SelectedColorIndex = GetSettingValue("SelectedColorIndex", planetId, 0);
		tacticalMapSettings.SettingsVisible = GetSettingValue("SettingsVisible", planetId, defaultValue: false);
		tacticalMapSettings.LabelMode = (TacticalMapControl.LabelMode)GetSettingValue("LabelMode", planetId, 2);
		tacticalMapSettings.WindowSize = new Vector2(GetSettingValue("WindowWidth", planetId, 1000f), GetSettingValue("WindowHeight", planetId, 800f));
		tacticalMapSettings.WindowPosition = new Vector2(GetSettingValue("WindowPositionX", planetId, -1f), GetSettingValue("WindowPositionY", planetId, -1f));
		TacticalMapSettings result = tacticalMapSettings;
		if (!string.IsNullOrEmpty(planetId))
		{
			CopyGlobalSettingsToMapIfNeeded(planetId);
		}
		return result;
	}

	private void CopyGlobalSettingsToMapIfNeeded(string planetId)
	{
		if (_modifiedSettings.Any((string text) => text.Contains("_" + planetId)))
		{
			return;
		}
		foreach (KeyValuePair<string, TacticalMapSettingRegistration> item in _currentSettings.Where<KeyValuePair<string, TacticalMapSettingRegistration>>((KeyValuePair<string, TacticalMapSettingRegistration> kvp) => !kvp.Key.Contains("_")).ToList())
		{
			item.Deconstruct(out var _, out var value);
			TacticalMapSettingRegistration tacticalMapSettingRegistration = value;
			if (tacticalMapSettingRegistration.Key != null)
			{
				string settingKey = GetSettingKey(tacticalMapSettingRegistration.Key, planetId);
				value = new TacticalMapSettingRegistration();
				value.Key = tacticalMapSettingRegistration.Key;
				value.Value = tacticalMapSettingRegistration.Value;
				value.PlanetId = planetId;
				TacticalMapSettingRegistration value2 = value;
				_currentSettings[settingKey] = value2;
				_modifiedSettings.Add(settingKey);
			}
		}
		SaveToUserData();
	}

	private T GetSettingValue<T>(string key, string? planetId, T defaultValue) where T : notnull
	{
		string settingKey = GetSettingKey(key, planetId);
		if (_currentSettings.TryGetValue(settingKey, out var value))
		{
			object value2 = value.Value;
			if (value2 is T)
			{
				return (T)value2;
			}
		}
		if (!string.IsNullOrEmpty(planetId))
		{
			string settingKey2 = GetSettingKey(key, null);
			if (_currentSettings.TryGetValue(settingKey2, out var value3))
			{
				object value2 = value3.Value;
				if (value2 is T)
				{
					return (T)value2;
				}
			}
		}
		return defaultValue;
	}

	private void SetSettingValue(string key, object value, string? planetId, bool markModified = true)
	{
		string settingKey = GetSettingKey(key, planetId);
		TacticalMapSettingRegistration tacticalMapSettingRegistration = new TacticalMapSettingRegistration();
		tacticalMapSettingRegistration.Key = key;
		tacticalMapSettingRegistration.Value = value;
		tacticalMapSettingRegistration.PlanetId = planetId;
		TacticalMapSettingRegistration value2 = tacticalMapSettingRegistration;
		_currentSettings[settingKey] = value2;
		if (markModified)
		{
			_modifiedSettings.Add(settingKey);
		}
	}

	public void SaveSettings(TacticalMapSettings settings, string? planetId = null)
	{
		SetSettingValue("ZoomFactor", settings.ZoomFactor, planetId);
		SetSettingValue("PanOffsetX", settings.PanOffset.X, planetId);
		SetSettingValue("PanOffsetY", settings.PanOffset.Y, planetId);
		SetSettingValue("BlipSizeMultiplier", settings.BlipSizeMultiplier, planetId);
		SetSettingValue("LineThickness", settings.LineThickness, planetId);
		SetSettingValue("SelectedColorIndex", settings.SelectedColorIndex, planetId);
		SetSettingValue("SettingsVisible", settings.SettingsVisible, planetId);
		SetSettingValue("LabelMode", (int)settings.LabelMode, planetId);
		SetSettingValue("WindowWidth", settings.WindowSize.X, planetId);
		SetSettingValue("WindowHeight", settings.WindowSize.Y, planetId);
		SetSettingValue("WindowPositionX", settings.WindowPosition.X, planetId);
		SetSettingValue("WindowPositionY", settings.WindowPosition.Y, planetId);
		try
		{
			SaveToUserData();
		}
		catch (Exception value)
		{
			_logger.Error($"TacticalMapSettingsManager: Failed to save to file: {value}");
		}
	}

	public void SaveViewSettings(float zoomFactor, Vector2 panOffset, string? planetId = null)
	{
		SetSettingValue("ZoomFactor", zoomFactor, planetId);
		SetSettingValue("PanOffsetX", panOffset.X, planetId);
		SetSettingValue("PanOffsetY", panOffset.Y, planetId);
		try
		{
			SaveToUserData();
		}
		catch (Exception value)
		{
			_logger.Error($"TacticalMapSettingsManager: Failed to save view settings: {value}");
		}
	}

	public void SaveWindowSizeAndPosition(Vector2 size, Vector2 position, string? planetId = null)
	{
		SetSettingValue("WindowWidth", size.X, planetId);
		SetSettingValue("WindowHeight", size.Y, planetId);
		SetSettingValue("WindowPositionX", position.X, planetId);
		SetSettingValue("WindowPositionY", position.Y, planetId);
		try
		{
			SaveToUserData();
		}
		catch (Exception value)
		{
			_logger.Error($"TacticalMapSettingsManager: Failed to save window settings: {value}");
		}
	}

	public void SaveWindowSize(Vector2 size, string? planetId = null)
	{
		SetSettingValue("WindowWidth", size.X, planetId);
		SetSettingValue("WindowHeight", size.Y, planetId);
		try
		{
			SaveToUserData();
		}
		catch (Exception value)
		{
			_logger.Error($"TacticalMapSettingsManager: Failed to save window size: {value}");
		}
	}

	public void SaveSingleSetting<T>(string key, T value, string? planetId = null) where T : notnull
	{
		SetSettingValue(key, value, planetId);
		try
		{
			SaveToUserData();
		}
		catch (Exception value2)
		{
			_logger.Error($"TacticalMapSettingsManager: Failed to save single setting: {value2}");
		}
	}

	public void SaveBlipSizeMultiplier(float value, string? planetId = null)
	{
		SaveSingleSetting("BlipSizeMultiplier", value, planetId);
	}

	public void SaveLineThickness(float value, string? planetId = null)
	{
		SaveSingleSetting("LineThickness", value, planetId);
	}

	public void SaveSelectedColorIndex(int value, string? planetId = null)
	{
		SaveSingleSetting("SelectedColorIndex", value, planetId);
	}

	public void SaveLabelMode(TacticalMapControl.LabelMode value, string? planetId = null)
	{
		SaveSingleSetting("LabelMode", (int)value, planetId);
	}

	public void SaveSettingsVisible(bool value, string? planetId = null)
	{
		SaveSingleSetting("SettingsVisible", value, planetId);
	}

	public void ResetSettingsFor(string key, string? planetId = null)
	{
		string settingKey = GetSettingKey(key, planetId);
		_modifiedSettings.Remove(settingKey);
		_currentSettings.Remove(settingKey);
		TacticalMapSettingRegistration value = _defaultRegistrations.FirstOrDefault((TacticalMapSettingRegistration r) => r.Key != null && r.Key == key && ((r.PlanetId == null && planetId == null) || (!string.IsNullOrEmpty(r.PlanetId) && !string.IsNullOrEmpty(planetId) && r.PlanetId == planetId)));
		if (!string.IsNullOrEmpty(value.Key))
		{
			_currentSettings[settingKey] = value;
		}
		SaveToUserData();
	}

	public void ResetAllSettings()
	{
		string[] array = _modifiedSettings.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split('_');
			string key = array2[0];
			string planetId = null;
			if (array2.Length > 1)
			{
				planetId = array2[1];
			}
			ResetSettingsFor(key, planetId);
		}
	}

	public bool IsSettingModified(string key, string? planetId = null)
	{
		return _modifiedSettings.Contains(GetSettingKey(key, planetId));
	}
}
