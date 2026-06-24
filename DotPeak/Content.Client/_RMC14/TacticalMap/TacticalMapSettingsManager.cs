// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.TacticalMap.TacticalMapSettingsManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

#nullable enable
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
    this._logger = Logger.GetSawmill("tactical_map_settings");
    this.Initialize();
  }

  private void Initialize()
  {
    this.RegisterDefaultSettings();
    ResPath file;
    // ISSUE: explicit constructor call
    ((ResPath) ref file).\u002Ector("/rmc_tactical_map_settings.yml");
    if (this._resourceMan.UserData.Exists(file))
    {
      try
      {
        this.LoadSettingsFile(file, false, true);
      }
      catch (Exception ex)
      {
        this._logger.Error("Failed to load user tactical map settings: " + ex?.ToString());
      }
    }
    if (!this._resourceMan.ContentFileExists(file))
      return;
    this.LoadSettingsFile(file, true);
  }

  private void RegisterDefaultSettings()
  {
    TacticalMapSettingRegistration[] settingRegistrationArray = new TacticalMapSettingRegistration[12];
    TacticalMapSettingRegistration settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "ZoomFactor";
    settingRegistration1.Value = (object) 1f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[0] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "PanOffsetX";
    settingRegistration1.Value = (object) 0.0f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[1] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "PanOffsetY";
    settingRegistration1.Value = (object) 0.0f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[2] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "BlipSizeMultiplier";
    settingRegistration1.Value = (object) 1f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[3] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "LineThickness";
    settingRegistration1.Value = (object) 2f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[4] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "SelectedColorIndex";
    settingRegistration1.Value = (object) 0;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[5] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "SettingsVisible";
    settingRegistration1.Value = (object) false;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[6] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "LabelMode";
    settingRegistration1.Value = (object) 2;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[7] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "WindowWidth";
    settingRegistration1.Value = (object) 1000f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[8] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "WindowHeight";
    settingRegistration1.Value = (object) 800f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[9] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "WindowPositionX";
    settingRegistration1.Value = (object) -1f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[10] = settingRegistration1;
    settingRegistration1 = new TacticalMapSettingRegistration();
    settingRegistration1.Key = "WindowPositionY";
    settingRegistration1.Value = (object) -1f;
    settingRegistration1.PlanetId = (string) null;
    settingRegistrationArray[11] = settingRegistration1;
    foreach (TacticalMapSettingRegistration settingRegistration2 in settingRegistrationArray)
    {
      this._defaultRegistrations.Add(settingRegistration2);
      if (!this._modifiedSettings.Contains(this.GetSettingKey(settingRegistration2.Key, settingRegistration2.PlanetId)))
        this._currentSettings[this.GetSettingKey(settingRegistration2.Key, settingRegistration2.PlanetId)] = settingRegistration2;
    }
  }

  private void LoadSettingsFile(ResPath file, bool defaultRegistration, bool userData = false)
  {
    TextReader textReader = !userData ? (TextReader) this._resourceMan.ContentFileReadText(file) : (TextReader) WritableDirProviderExt.OpenText(this._resourceMan.UserData, file);
    using (textReader)
    {
      try
      {
        MappingDataNode root = (MappingDataNode) DataNodeParser.ParseYamlStream(textReader).First<DataNodeDocument>().Root;
        DataNode dataNode1;
        if (!root.TryGet("settings", ref dataNode1))
          return;
        if (dataNode1 == null)
        {
          this._logger.Warning("Settings node is null, skipping settings load");
        }
        else
        {
          TacticalMapSettingRegistration[] settingRegistrationArray;
          try
          {
            settingRegistrationArray = this._serialization.Read<TacticalMapSettingRegistration[]>(dataNode1, (ISerializationContext) null, false, (ISerializationManager.InstantiationDelegate<TacticalMapSettingRegistration[]>) null, false);
          }
          catch (Exception ex)
          {
            this._logger.Error($"Failed to parse settings array: {ex}");
            return;
          }
          if (settingRegistrationArray == null)
          {
            this._logger.Warning("Parsed settings array is null, skipping settings load");
          }
          else
          {
            foreach (TacticalMapSettingRegistration settingRegistration in settingRegistrationArray)
            {
              if (settingRegistration.Key == null || string.IsNullOrEmpty(settingRegistration.Key))
                this._logger.Warning("Skipping setting with null/empty key");
              else if (settingRegistration.Value == null)
              {
                this._logger.Warning($"Skipping setting '{settingRegistration.Key}' with null value");
              }
              else
              {
                string settingKey = this.GetSettingKey(settingRegistration.Key, settingRegistration.PlanetId);
                if (defaultRegistration)
                {
                  this._defaultRegistrations.Add(settingRegistration);
                  if (this._modifiedSettings.Contains(settingKey))
                    continue;
                }
                this._currentSettings[settingKey] = settingRegistration;
                if (!defaultRegistration)
                  this._modifiedSettings.Add(settingKey);
              }
            }
            DataNode dataNode2;
            if (defaultRegistration || !root.TryGet("unsetSettings", ref dataNode2))
              return;
            if (dataNode2 == null)
              return;
            try
            {
              string[] strArray = this._serialization.Read<string[]>(dataNode2, (ISerializationContext) null, false, (ISerializationManager.InstantiationDelegate<string[]>) null, false);
              if (strArray == null)
                return;
              foreach (string key in strArray)
              {
                if (!string.IsNullOrEmpty(key))
                {
                  this._modifiedSettings.Add(key);
                  this._currentSettings.Remove(key);
                }
              }
            }
            catch (Exception ex)
            {
              this._logger.Error($"Failed to parse unsetSettings array: {ex}");
            }
          }
        }
      }
      catch (Exception ex)
      {
        this._logger.Error($"Failed to parse settings file {file}: {ex}");
        if (defaultRegistration)
          return;
        this._logger.Info("Continuing with default settings due to parse failure");
      }
    }
  }

  private string GetSettingKey(string key, string? planetId)
  {
    return !string.IsNullOrEmpty(planetId) ? $"{key}_{planetId}" : key;
  }

  public void SaveToUserData()
  {
    ResPath resPath;
    // ISSUE: explicit constructor call
    ((ResPath) ref resPath).\u002Ector("/rmc_tactical_map_settings.yml");
    try
    {
      using (StreamWriter streamWriter = WritableDirProviderExt.OpenWriteText(this._resourceMan.UserData, resPath))
      {
        streamWriter.WriteLine("version: \"1\"");
        streamWriter.WriteLine("settings:");
        foreach (TacticalMapSettingRegistration settingRegistration in this._modifiedSettings.Where<string>((Func<string, bool>) (key => this._currentSettings.ContainsKey(key))).Select<string, TacticalMapSettingRegistration>((Func<string, TacticalMapSettingRegistration>) (key => this._currentSettings[key])).Where<TacticalMapSettingRegistration>((Func<TacticalMapSettingRegistration, bool>) (setting => setting.Key != null)))
        {
          streamWriter.WriteLine($"  - Key: \"{settingRegistration.Key}\"");
          streamWriter.WriteLine("    Value: " + this.FormatValueForYaml(settingRegistration.Value));
          if (!string.IsNullOrEmpty(settingRegistration.PlanetId))
            streamWriter.WriteLine($"    PlanetId: \"{settingRegistration.PlanetId}\"");
          else
            streamWriter.WriteLine("    PlanetId: null");
        }
        streamWriter.WriteLine("unsetSettings:");
        foreach (string str in this._modifiedSettings.Where<string>((Func<string, bool>) (key => !this._currentSettings.ContainsKey(key))))
          streamWriter.WriteLine($"  - \"{str}\"");
      }
    }
    catch (Exception ex)
    {
      this._logger.Error($"Failed to save tactical map settings: {ex}");
    }
  }

  private string FormatValueForYaml(object? value)
  {
    string str1;
    switch (value)
    {
      case null:
        return "null";
      case string str2:
        str1 = $"\"{str2.Replace("\"", "\\\"")}\"";
        break;
      case bool flag:
        str1 = flag.ToString().ToLower();
        break;
      case float num1:
        str1 = num1.ToString("F6");
        break;
      case int num2:
        str1 = num2.ToString();
        break;
      default:
        str1 = $"\"{value.ToString()}\"";
        break;
    }
    return str1;
  }

  public TacticalMapSettings LoadSettings(string? planetId = null)
  {
    TacticalMapSettings tacticalMapSettings = new TacticalMapSettings()
    {
      ZoomFactor = this.GetSettingValue<float>("ZoomFactor", planetId, 1f),
      PanOffset = new Vector2(this.GetSettingValue<float>("PanOffsetX", planetId, 0.0f), this.GetSettingValue<float>("PanOffsetY", planetId, 0.0f)),
      BlipSizeMultiplier = this.GetSettingValue<float>("BlipSizeMultiplier", planetId, 1f),
      LineThickness = this.GetSettingValue<float>("LineThickness", planetId, 2f),
      SelectedColorIndex = this.GetSettingValue<int>("SelectedColorIndex", planetId, 0),
      SettingsVisible = this.GetSettingValue<bool>("SettingsVisible", planetId, false),
      LabelMode = (TacticalMapControl.LabelMode) this.GetSettingValue<int>("LabelMode", planetId, 2),
      WindowSize = new Vector2(this.GetSettingValue<float>("WindowWidth", planetId, 1000f), this.GetSettingValue<float>("WindowHeight", planetId, 800f)),
      WindowPosition = new Vector2(this.GetSettingValue<float>("WindowPositionX", planetId, -1f), this.GetSettingValue<float>("WindowPositionY", planetId, -1f))
    };
    if (string.IsNullOrEmpty(planetId))
      return tacticalMapSettings;
    this.CopyGlobalSettingsToMapIfNeeded(planetId);
    return tacticalMapSettings;
  }

  private void CopyGlobalSettingsToMapIfNeeded(string planetId)
  {
    if (this._modifiedSettings.Any<string>((Func<string, bool>) (key => key.Contains("_" + planetId))))
      return;
    foreach ((string _, TacticalMapSettingRegistration settingRegistration1) in this._currentSettings.Where<KeyValuePair<string, TacticalMapSettingRegistration>>((Func<KeyValuePair<string, TacticalMapSettingRegistration>, bool>) (kvp => !kvp.Key.Contains("_"))).ToList<KeyValuePair<string, TacticalMapSettingRegistration>>())
    {
      TacticalMapSettingRegistration settingRegistration2 = settingRegistration1;
      if (settingRegistration2.Key != null)
      {
        string settingKey = this.GetSettingKey(settingRegistration2.Key, planetId);
        settingRegistration1 = new TacticalMapSettingRegistration();
        settingRegistration1.Key = settingRegistration2.Key;
        settingRegistration1.Value = settingRegistration2.Value;
        settingRegistration1.PlanetId = planetId;
        TacticalMapSettingRegistration settingRegistration3 = settingRegistration1;
        this._currentSettings[settingKey] = settingRegistration3;
        this._modifiedSettings.Add(settingKey);
      }
    }
    this.SaveToUserData();
  }

  private T GetSettingValue<T>(string key, string? planetId, T defaultValue) where T : notnull
  {
    TacticalMapSettingRegistration settingRegistration1;
    if (this._currentSettings.TryGetValue(this.GetSettingKey(key, planetId), out settingRegistration1) && settingRegistration1.Value is T settingValue)
      return settingValue;
    TacticalMapSettingRegistration settingRegistration2;
    return !string.IsNullOrEmpty(planetId) && this._currentSettings.TryGetValue(this.GetSettingKey(key, (string) null), out settingRegistration2) && settingRegistration2.Value is T obj ? obj : defaultValue;
  }

  private void SetSettingValue(string key, object value, string? planetId, bool markModified = true)
  {
    string settingKey = this.GetSettingKey(key, planetId);
    TacticalMapSettingRegistration settingRegistration = new TacticalMapSettingRegistration()
    {
      Key = key,
      Value = value,
      PlanetId = planetId
    };
    this._currentSettings[settingKey] = settingRegistration;
    if (!markModified)
      return;
    this._modifiedSettings.Add(settingKey);
  }

  public void SaveSettings(TacticalMapSettings settings, string? planetId = null)
  {
    this.SetSettingValue("ZoomFactor", (object) settings.ZoomFactor, planetId);
    this.SetSettingValue("PanOffsetX", (object) settings.PanOffset.X, planetId);
    this.SetSettingValue("PanOffsetY", (object) settings.PanOffset.Y, planetId);
    this.SetSettingValue("BlipSizeMultiplier", (object) settings.BlipSizeMultiplier, planetId);
    this.SetSettingValue("LineThickness", (object) settings.LineThickness, planetId);
    this.SetSettingValue("SelectedColorIndex", (object) settings.SelectedColorIndex, planetId);
    this.SetSettingValue("SettingsVisible", (object) settings.SettingsVisible, planetId);
    this.SetSettingValue("LabelMode", (object) (int) settings.LabelMode, planetId);
    this.SetSettingValue("WindowWidth", (object) settings.WindowSize.X, planetId);
    this.SetSettingValue("WindowHeight", (object) settings.WindowSize.Y, planetId);
    this.SetSettingValue("WindowPositionX", (object) settings.WindowPosition.X, planetId);
    this.SetSettingValue("WindowPositionY", (object) settings.WindowPosition.Y, planetId);
    try
    {
      this.SaveToUserData();
    }
    catch (Exception ex)
    {
      this._logger.Error($"TacticalMapSettingsManager: Failed to save to file: {ex}");
    }
  }

  public void SaveViewSettings(float zoomFactor, Vector2 panOffset, string? planetId = null)
  {
    this.SetSettingValue("ZoomFactor", (object) zoomFactor, planetId);
    this.SetSettingValue("PanOffsetX", (object) panOffset.X, planetId);
    this.SetSettingValue("PanOffsetY", (object) panOffset.Y, planetId);
    try
    {
      this.SaveToUserData();
    }
    catch (Exception ex)
    {
      this._logger.Error($"TacticalMapSettingsManager: Failed to save view settings: {ex}");
    }
  }

  public void SaveWindowSizeAndPosition(Vector2 size, Vector2 position, string? planetId = null)
  {
    this.SetSettingValue("WindowWidth", (object) size.X, planetId);
    this.SetSettingValue("WindowHeight", (object) size.Y, planetId);
    this.SetSettingValue("WindowPositionX", (object) position.X, planetId);
    this.SetSettingValue("WindowPositionY", (object) position.Y, planetId);
    try
    {
      this.SaveToUserData();
    }
    catch (Exception ex)
    {
      this._logger.Error($"TacticalMapSettingsManager: Failed to save window settings: {ex}");
    }
  }

  public void SaveWindowSize(Vector2 size, string? planetId = null)
  {
    this.SetSettingValue("WindowWidth", (object) size.X, planetId);
    this.SetSettingValue("WindowHeight", (object) size.Y, planetId);
    try
    {
      this.SaveToUserData();
    }
    catch (Exception ex)
    {
      this._logger.Error($"TacticalMapSettingsManager: Failed to save window size: {ex}");
    }
  }

  public void SaveSingleSetting<T>(string key, T value, string? planetId = null) where T : notnull
  {
    this.SetSettingValue(key, (object) value, planetId);
    try
    {
      this.SaveToUserData();
    }
    catch (Exception ex)
    {
      this._logger.Error($"TacticalMapSettingsManager: Failed to save single setting: {ex}");
    }
  }

  public void SaveBlipSizeMultiplier(float value, string? planetId = null)
  {
    this.SaveSingleSetting<float>("BlipSizeMultiplier", value, planetId);
  }

  public void SaveLineThickness(float value, string? planetId = null)
  {
    this.SaveSingleSetting<float>("LineThickness", value, planetId);
  }

  public void SaveSelectedColorIndex(int value, string? planetId = null)
  {
    this.SaveSingleSetting<int>("SelectedColorIndex", value, planetId);
  }

  public void SaveLabelMode(TacticalMapControl.LabelMode value, string? planetId = null)
  {
    this.SaveSingleSetting<int>("LabelMode", (int) value, planetId);
  }

  public void SaveSettingsVisible(bool value, string? planetId = null)
  {
    this.SaveSingleSetting<bool>("SettingsVisible", value, planetId);
  }

  public void ResetSettingsFor(string key, string? planetId = null)
  {
    string settingKey = this.GetSettingKey(key, planetId);
    this._modifiedSettings.Remove(settingKey);
    this._currentSettings.Remove(settingKey);
    TacticalMapSettingRegistration settingRegistration = this._defaultRegistrations.FirstOrDefault<TacticalMapSettingRegistration>((Func<TacticalMapSettingRegistration, bool>) (r =>
    {
      if (r.Key == null || !(r.Key == key))
        return false;
      if (r.PlanetId == null && planetId == null)
        return true;
      return !string.IsNullOrEmpty(r.PlanetId) && !string.IsNullOrEmpty(planetId) && r.PlanetId == planetId;
    }));
    if (!string.IsNullOrEmpty(settingRegistration.Key))
      this._currentSettings[settingKey] = settingRegistration;
    this.SaveToUserData();
  }

  public void ResetAllSettings()
  {
    foreach (string str in this._modifiedSettings.ToArray<string>())
    {
      string[] strArray = str.Split('_');
      string key = strArray[0];
      string planetId = (string) null;
      if (strArray.Length > 1)
        planetId = strArray[1];
      this.ResetSettingsFor(key, planetId);
    }
  }

  public bool IsSettingModified(string key, string? planetId = null)
  {
    return this._modifiedSettings.Contains(this.GetSettingKey(key, planetId));
  }
}
