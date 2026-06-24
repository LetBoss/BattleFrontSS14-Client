// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.ConfigurationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Nett;
using Robust.Shared.Analyzers;
using Robust.Shared.Collections;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

#nullable enable
namespace Robust.Shared.Configuration;

[Virtual]
internal class ConfigurationManager : IConfigurationManagerInternal, IConfigurationManager
{
  [Dependency]
  private readonly IGameTiming _gameTiming;
  [Dependency]
  private readonly ILogManager _logManager;
  private const char TABLE_DELIMITER = '.';
  protected readonly Dictionary<string, ConfigurationManager.ConfigVar> _configVars = new Dictionary<string, ConfigurationManager.ConfigVar>();
  private ConfigurationManager.ConfigFileStorage? _configFile;
  protected bool _isServer;
  protected readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
  private ISawmill _sawmill;

  public event Action<CVarChangeInfo>? OnCVarValueChanged;

  public void Initialize(bool isServer)
  {
    this._isServer = isServer;
    this._sawmill = this._logManager.GetSawmill("cfg");
  }

  public virtual void Shutdown()
  {
    using (this.Lock.WriteGuard())
    {
      this._configVars.Clear();
      this._configFile = (ConfigurationManager.ConfigFileStorage) null;
    }
  }

  public HashSet<string> LoadFromTomlStream(Stream file)
  {
    TomlTable table;
    try
    {
      table = Toml.ReadStream(file);
    }
    catch (Exception ex)
    {
      this._sawmill.Error("Unable to load configuration from table:\n{0}", (object) ex);
      return new HashSet<string>();
    }
    return this.LoadFromTomlTable(table);
  }

  private HashSet<string> LoadFromTomlTable(TomlTable table)
  {
    HashSet<string> stringSet = new HashSet<string>();
    ValueList<ConfigurationManager.ValueChangedInvoke> callbackEvents = new ValueList<ConfigurationManager.ValueChangedInvoke>();
    try
    {
      using (this.Lock.WriteGuard())
      {
        foreach ((string cvar, object value) in this.ParseCVarValuesFromToml(table))
        {
          stringSet.Add(cvar);
          this.LoadParsedVar(cvar, value, ref callbackEvents);
        }
      }
    }
    finally
    {
      this.RunDeferredInvokeCallbacks(in callbackEvents);
    }
    return stringSet;
  }

  private void RunDeferredInvokeCallbacks(
    in ValueList<ConfigurationManager.ValueChangedInvoke> callbackEvents)
  {
    foreach (ConfigurationManager.ValueChangedInvoke invoke in callbackEvents)
      this.InvokeValueChanged(in invoke);
  }

  private void LoadParsedVar(
    string cvar,
    object value,
    ref ValueList<ConfigurationManager.ValueChangedInvoke> changedInvokes)
  {
    ConfigurationManager.ConfigVar configVar;
    if (this._configVars.TryGetValue(cvar, out configVar))
    {
      object configVarValue = ConfigurationManager.GetConfigVarValue(configVar);
      object newValue = value;
      if (configVar.Type != value.GetType())
      {
        try
        {
          newValue = ConfigurationManager.ConvertToCVarType(value, configVar.Type);
        }
        catch
        {
          this._sawmill.Error($"Parsed cvar does not match registered cvar type. Name: {cvar}. Code Type: {configVar.Type}. Parsed type: {value.GetType()}");
          return;
        }
      }
      changedInvokes.Add(this.SetupInvokeValueChanged(configVar, newValue, configVarValue));
      configVar.Value = newValue;
    }
    else
      configVar = this.AddUnregisteredCVar(cvar, value);
    configVar.ConfigModified = true;
  }

  private ConfigurationManager.ConfigVar AddUnregisteredCVar(string name, object value)
  {
    ConfigurationManager.ConfigVar configVar = new ConfigurationManager.ConfigVar(name, (object) null, CVar.NONE)
    {
      Value = value
    };
    this._configVars.Add(name, configVar);
    return configVar;
  }

  public HashSet<string> LoadDefaultsFromTomlStream(Stream stream)
  {
    TomlTable tblRoot = Toml.ReadStream(stream);
    HashSet<string> stringSet = new HashSet<string>();
    ValueList<ConfigurationManager.ValueChangedInvoke> valueList = new ValueList<ConfigurationManager.ValueChangedInvoke>();
    using (this.Lock.WriteGuard())
    {
      foreach ((string str, object value) in this.ParseCVarValuesFromToml(tblRoot))
      {
        ConfigurationManager.ConfigVar configVar;
        if (!this._configVars.TryGetValue(str, out configVar) || !configVar.Registered)
        {
          this._sawmill.Error($"Trying to set unregistered variable '{str}'");
        }
        else
        {
          object newValue = value;
          if (configVar.Type != value.GetType())
          {
            try
            {
              newValue = ConfigurationManager.ConvertToCVarType(value, configVar.Type);
            }
            catch
            {
              this._sawmill.Error($"Override TOML parsed cvar does not match registered cvar type. Name: {str}. Code Type: {configVar.Type}. Toml type: {value.GetType()}");
              continue;
            }
          }
          if (configVar.OverrideValue == null && configVar.Value == null)
          {
            object configVarValue = ConfigurationManager.GetConfigVarValue(configVar);
            valueList.Add(this.SetupInvokeValueChanged(configVar, newValue, configVarValue));
          }
          configVar.DefaultValue = newValue;
        }
      }
    }
    foreach (ConfigurationManager.ValueChangedInvoke invoke in valueList)
      this.InvokeValueChanged(in invoke);
    return stringSet;
  }

  public HashSet<string> LoadFromFile(string configFile)
  {
    try
    {
      HashSet<string> stringSet;
      using (FileStream file = File.OpenRead(configFile))
        stringSet = this.LoadFromTomlStream((Stream) file);
      this.SetSaveFile(configFile);
      this.ApplyRollback();
      this._sawmill.Info("Configuration loaded from file");
      return stringSet;
    }
    catch (Exception ex)
    {
      this._sawmill.Error("Unable to load configuration file:\n{0}", (object) ex);
      return new HashSet<string>(0);
    }
  }

  public void SetSaveFile(string configFile)
  {
    this._configFile = (ConfigurationManager.ConfigFileStorage) new ConfigurationManager.ConfigFileStorageDisk()
    {
      Path = configFile
    };
  }

  public void SetVirtualConfig()
  {
    this._configFile = (ConfigurationManager.ConfigFileStorage) new ConfigurationManager.ConfigFileStorageVirtual();
  }

  public void CheckUnusedCVars()
  {
    if (!this.GetCVar<bool>(CVars.CfgCheckUnused))
      return;
    using (this.Lock.ReadGuard())
    {
      foreach (ConfigurationManager.ConfigVar configVar in this._configVars.Values)
      {
        if (!configVar.Registered)
          this._sawmill.Warning("Unknown CVar found (typo in config?): {CVar}", (object) configVar.Name);
      }
    }
  }

  public void SaveToTomlStream(Stream stream, IEnumerable<string> cvars)
  {
    Toml.WriteStream(this.SaveToTomlTable(cvars), stream);
  }

  private TomlTable SaveToTomlTable(IEnumerable<string> cvars, Func<string, object>? overrideValue = null)
  {
    TomlTable tomlTable1 = Toml.Create();
    using (this.Lock.ReadGuard())
    {
      foreach (string cvar in cvars)
      {
        ConfigurationManager.ConfigVar configVar;
        if (this._configVars.TryGetValue(cvar, out configVar))
        {
          object obj;
          if (overrideValue != null)
          {
            obj = overrideValue(cvar);
          }
          else
          {
            obj = configVar.Value;
            if (obj == null && configVar.Registered)
              obj = configVar.DefaultValue;
          }
          if (obj == null)
          {
            this._sawmill.Error($"CVar {cvar} has no value or default value, was the default value registered as null?");
          }
          else
          {
            int length = cvar.LastIndexOf('.');
            string[] strArray = cvar.Substring(0, length).Split('.');
            string str1 = cvar.Substring(length + 1);
            TomlTable tomlTable2 = tomlTable1;
            foreach (string str2 in strArray)
            {
              TomlObject added;
              if (!tomlTable2.TryGetValue(str2, ref added))
                added = (TomlObject) TomlObjectFactory.Add<TomlObject>(tomlTable2, str2, (IDictionary<string, TomlObject>) new Dictionary<string, TomlObject>(), (TomlTable.TableTypes) 0).Added;
              tomlTable2 = added is TomlTable tomlTable3 ? tomlTable3 : throw new InvalidConfigurationException($"[CFG] Object {str2} is being used like a table, but it is a {added}. Are your CVar names formed properly?");
            }
            switch (obj)
            {
              case Enum @enum:
                TomlObjectFactory.Add(tomlTable2, str1, (int) @enum);
                continue;
              case int num1:
                TomlObjectFactory.Add(tomlTable2, str1, num1);
                continue;
              case long num2:
                TomlObjectFactory.Add(tomlTable2, str1, num2);
                continue;
              case bool flag:
                TomlObjectFactory.Add(tomlTable2, str1, flag);
                continue;
              case string str3:
                TomlObjectFactory.Add(tomlTable2, str1, str3);
                continue;
              case float num3:
                TomlObjectFactory.Add(tomlTable2, str1, num3);
                continue;
              case double num4:
                TomlObjectFactory.Add(tomlTable2, str1, num4);
                continue;
              default:
                this._sawmill.Warning($"Cannot serialize '{cvar}', unsupported type.");
                continue;
            }
          }
        }
      }
    }
    return tomlTable1;
  }

  public void SaveToFile()
  {
    if (this._configFile == null)
    {
      this._sawmill.Warning("Cannot save the config file, because one was never loaded.");
    }
    else
    {
      try
      {
        IEnumerable<string> cvars = this._configVars.Where<KeyValuePair<string, ConfigurationManager.ConfigVar>>((Func<KeyValuePair<string, ConfigurationManager.ConfigVar>, bool>) (x =>
        {
          if (x.Value.ConfigModified)
            return true;
          return (x.Value.Flags & CVar.ARCHIVE) != CVar.NONE && x.Value.Value != null && !x.Value.Value.Equals(x.Value.DefaultValue);
        })).Select<KeyValuePair<string, ConfigurationManager.ConfigVar>, string>((Func<KeyValuePair<string, ConfigurationManager.ConfigVar>, string>) (x => x.Key));
        MemoryStream memoryStream = new MemoryStream();
        this.SaveToTomlStream((Stream) memoryStream, cvars);
        memoryStream.Position = 0L;
        switch (this._configFile)
        {
          case ConfigurationManager.ConfigFileStorageDisk configFileStorageDisk:
            using (FileStream destination = File.Create(configFileStorageDisk.Path))
            {
              memoryStream.CopyTo((Stream) destination);
              break;
            }
          case ConfigurationManager.ConfigFileStorageVirtual fileStorageVirtual:
            fileStorageVirtual.Stream.SetLength(0L);
            memoryStream.CopyTo((Stream) fileStorageVirtual.Stream);
            break;
          default:
            throw new UnreachableException();
        }
        this._sawmill.Info($"config saved to '{this._configFile}'.");
      }
      catch (Exception ex)
      {
        this._sawmill.Warning($"Cannot save the config file '{this._configFile}'.\n {ex}");
      }
    }
  }

  public void RegisterCVar<T>(string name, T defaultValue, CVar flags = CVar.NONE, Action<T>? onValueChanged = null) where T : notnull
  {
    this.RegisterCVar(name, typeof (T), (object) defaultValue, flags);
    if (onValueChanged == null)
      return;
    this.OnValueChanged<T>(name, onValueChanged, false);
  }

  private void RegisterCVar(string name, Type type, object defaultValue, CVar flags)
  {
    CVar cvar = this._isServer ? CVar.CLIENTONLY : CVar.SERVERONLY;
    if ((flags & cvar) != CVar.NONE)
      return;
    using (this.Lock.WriteGuard())
    {
      ConfigurationManager.ConfigVar configVar1;
      if (this._configVars.TryGetValue(name, out configVar1))
      {
        if (configVar1.Registered)
          this._sawmill.Error($"The variable '{name}' has already been registered.");
        if (configVar1.Value != null)
        {
          if (type != configVar1.Value.GetType())
          {
            try
            {
              configVar1.Value = ConfigurationManager.ConvertToCVarType(configVar1.Value, type);
            }
            catch
            {
              this._sawmill.Error($"TOML parsed cvar does not match registered cvar type. Name: {name}. Code Type: {type.Name}. Toml type: {configVar1.Value.GetType().Name}");
              return;
            }
          }
        }
        configVar1.DefaultValue = defaultValue;
        configVar1.Flags = flags;
        configVar1.Register();
        if (configVar1.OverrideValue == null)
          return;
        configVar1.OverrideValueParsed = ConfigurationManager.ParseOverrideValue(configVar1.OverrideValue, type);
      }
      else
      {
        ConfigurationManager.ConfigVar configVar2 = new ConfigurationManager.ConfigVar(name, defaultValue, flags);
        configVar2.Register();
        this._configVars.Add(name, configVar2);
      }
    }
  }

  public void OnValueChanged<T>(CVarDef<T> cVar, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
  {
    this.OnValueChanged<T>(cVar.Name, onValueChanged, invokeImmediately);
  }

  public void OnValueChanged<T>(string name, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
  {
    using (this.Lock.WriteGuard())
      this._configVars[name].ValueChanged.AddInPlace((ConfigurationManager.ValueChangedDelegate) ((object value, in CVarChangeInfo _) => onValueChanged((T) value)), (object) onValueChanged);
    if (!invokeImmediately)
      return;
    onValueChanged(this.GetCVar<T>(name));
  }

  public void UnsubValueChanged<T>(CVarDef<T> cVar, Action<T> onValueChanged) where T : notnull
  {
    this.UnsubValueChanged<T>(cVar.Name, onValueChanged);
  }

  public void UnsubValueChanged<T>(string name, Action<T> onValueChanged) where T : notnull
  {
    using (this.Lock.WriteGuard())
      this._configVars[name].ValueChanged.RemoveInPlace((object) onValueChanged);
  }

  public void OnValueChanged<T>(
    CVarDef<T> cVar,
    CVarChanged<T> onValueChanged,
    bool invokeImmediately = false)
    where T : notnull
  {
    this.OnValueChanged<T>(cVar.Name, onValueChanged, invokeImmediately);
  }

  public void OnValueChanged<T>(string name, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull
  {
    object configVarValue;
    using (this.Lock.WriteGuard())
    {
      ConfigurationManager.ConfigVar configVar = this._configVars[name];
      configVarValue = ConfigurationManager.GetConfigVarValue(configVar);
      configVar.ValueChanged.AddInPlace((ConfigurationManager.ValueChangedDelegate) ((object value, in CVarChangeInfo info) => onValueChanged((T) value, in info)), (object) onValueChanged);
    }
    if (!invokeImmediately)
      return;
    onValueChanged(this.GetCVar<T>(name), new CVarChangeInfo(name, this._gameTiming.CurTick, configVarValue, configVarValue));
  }

  public void UnsubValueChanged<T>(CVarDef<T> cVar, CVarChanged<T> onValueChanged) where T : notnull
  {
    this.UnsubValueChanged<T>(cVar.Name, onValueChanged);
  }

  public void UnsubValueChanged<T>(string name, CVarChanged<T> onValueChanged) where T : notnull
  {
    using (this.Lock.WriteGuard())
      this._configVars[name].ValueChanged.RemoveInPlace((object) onValueChanged);
  }

  public void LoadCVarsFromAssembly(Assembly assembly)
  {
    foreach (Type containingType in ((IEnumerable<Type>) assembly.GetTypes()).Where<Type>((Func<Type, bool>) (p => Attribute.IsDefined((MemberInfo) p, typeof (CVarDefsAttribute)))))
      this.LoadCVarsFromType(containingType);
  }

  public void LoadCVarsFromType(Type containingType)
  {
    foreach (FieldInfo field in containingType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
    {
      Type fieldType = field.FieldType;
      if (fieldType.IsGenericType && !(fieldType.GetGenericTypeDefinition() != typeof (CVarDef<>)))
      {
        Type genericArgument = fieldType.GetGenericArguments()[0];
        CVarDef cvarDef = field.IsInitOnly ? (CVarDef) field.GetValue((object) null) : throw new InvalidOperationException($"Found CVarDef '{field.Name}' on '{field.DeclaringType?.FullName}' that is not readonly. Please mark it as readonly.");
        if (cvarDef == null)
          throw new InvalidOperationException($"CVarDef '{field.Name}' on '{field.DeclaringType?.FullName}' is null.");
        this.RegisterCVar(cvarDef.Name, genericArgument, cvarDef.DefaultValue, cvarDef.Flags);
      }
    }
  }

  public bool IsCVarRegistered(string name)
  {
    using (this.Lock.ReadGuard())
    {
      ConfigurationManager.ConfigVar configVar;
      return this._configVars.TryGetValue(name, out configVar) && configVar.Registered;
    }
  }

  public CVar GetCVarFlags(string name)
  {
    using (this.Lock.ReadGuard())
      return this._configVars[name].Flags;
  }

  public IEnumerable<string> GetRegisteredCVars()
  {
    using (this.Lock.ReadGuard())
      return (IEnumerable<string>) this._configVars.Where<KeyValuePair<string, ConfigurationManager.ConfigVar>>((Func<KeyValuePair<string, ConfigurationManager.ConfigVar>, bool>) (c => c.Value.Registered)).Select<KeyValuePair<string, ConfigurationManager.ConfigVar>, string>((Func<KeyValuePair<string, ConfigurationManager.ConfigVar>, string>) (p => p.Key)).ToArray<string>();
  }

  public virtual void SetCVar(string name, object value, bool force = false)
  {
    this.SetCVarInternal(name, value, this._gameTiming.CurTick);
  }

  protected void SetCVarInternal(string name, object value, GameTick intendedTick)
  {
    ConfigurationManager.ValueChangedInvoke? nullable = new ConfigurationManager.ValueChangedInvoke?();
    using (this.Lock.WriteGuard())
    {
      ConfigurationManager.ConfigVar configVar;
      if (!this._configVars.TryGetValue(name, out configVar) || !configVar.Registered)
        throw new InvalidConfigurationException($"Trying to set unregistered variable '{name}'");
      if (!object.Equals(configVar.OverrideValueParsed ?? configVar.Value, value))
      {
        object configVarValue = ConfigurationManager.GetConfigVarValue(configVar);
        nullable = new ConfigurationManager.ValueChangedInvoke?(this.SetupInvokeValueChanged(configVar, value, configVarValue, new GameTick?(intendedTick)));
        configVar.OverrideValue = (string) null;
        configVar.OverrideValueParsed = (object) null;
        configVar.Value = value;
      }
    }
    if (!nullable.HasValue)
      return;
    this.InvokeValueChanged(nullable.Value);
  }

  public void SetCVar<T>(CVarDef<T> def, T value, bool force = false) where T : notnull
  {
    this.SetCVar(def.Name, (object) value, force);
  }

  public void OverrideDefault(string name, object value)
  {
    ConfigurationManager.ValueChangedInvoke? nullable = new ConfigurationManager.ValueChangedInvoke?();
    using (this.Lock.WriteGuard())
    {
      ConfigurationManager.ConfigVar configVar;
      if (!this._configVars.TryGetValue(name, out configVar) || !configVar.Registered)
        throw new InvalidConfigurationException($"Trying to set unregistered variable '{name}'");
      if (configVar.OverrideValue == null && configVar.Value == null)
      {
        object configVarValue = ConfigurationManager.GetConfigVarValue(configVar);
        nullable = new ConfigurationManager.ValueChangedInvoke?(this.SetupInvokeValueChanged(configVar, value, configVarValue));
      }
      configVar.DefaultValue = value;
    }
    if (!nullable.HasValue)
      return;
    this.InvokeValueChanged(nullable.Value);
  }

  public void OverrideDefault<T>(CVarDef<T> def, T value) where T : notnull
  {
    this.OverrideDefault(def.Name, (object) value);
  }

  public object GetCVar(string name)
  {
    using (this.Lock.ReadGuard())
    {
      ConfigurationManager.ConfigVar cVar;
      if (this._configVars.TryGetValue(name, out cVar) && cVar.Registered)
        return ConfigurationManager.GetConfigVarValue(cVar);
      throw new InvalidConfigurationException($"Trying to get unregistered variable '{name}'");
    }
  }

  public T GetCVar<T>(string name) => (T) this.GetCVar(name);

  public T GetCVar<T>(CVarDef<T> def) where T : notnull => this.GetCVar<T>(def.Name);

  public Type GetCVarType(string name)
  {
    using (this.Lock.ReadGuard())
    {
      ConfigurationManager.ConfigVar configVar;
      if (!this._configVars.TryGetValue(name, out configVar) || !configVar.Registered)
        throw new InvalidConfigurationException($"Trying to get type of unregistered variable '{name}'");
      return configVar.Type;
    }
  }

  protected static object GetConfigVarValue(ConfigurationManager.ConfigVar cVar)
  {
    return cVar.OverrideValueParsed ?? cVar.Value ?? cVar.DefaultValue;
  }

  public void OverrideConVars(IEnumerable<(string key, string value)> cVars)
  {
    ValueList<ConfigurationManager.ValueChangedInvoke> valueList = new ValueList<ConfigurationManager.ValueChangedInvoke>();
    using (this.Lock.WriteGuard())
    {
      foreach ((string str, string value) in cVars)
      {
        ConfigurationManager.ConfigVar configVar1;
        if (this._configVars.TryGetValue(str, out configVar1))
        {
          configVar1.OverrideValue = value;
          if (configVar1.Registered)
          {
            object overrideValue = ConfigurationManager.ParseOverrideValue(value, configVar1.Type);
            object configVarValue = ConfigurationManager.GetConfigVarValue(configVar1);
            valueList.Add(this.SetupInvokeValueChanged(configVar1, overrideValue, configVarValue));
            configVar1.OverrideValueParsed = overrideValue;
          }
        }
        else
        {
          ConfigurationManager.ConfigVar configVar2 = new ConfigurationManager.ConfigVar(str, (object) null, CVar.NONE)
          {
            OverrideValue = value
          };
          this._configVars.Add(str, configVar2);
        }
      }
    }
    foreach (ConfigurationManager.ValueChangedInvoke invoke in valueList)
      this.InvokeValueChanged(in invoke);
  }

  private static object ParseOverrideValue(string value, Type? type)
  {
    if (type == typeof (int))
      return (object) int.Parse(value);
    if (type == typeof (bool))
      return (object) bool.Parse(value);
    if (type == typeof (float))
      return (object) float.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture);
    if ((object) type != null && type.IsEnum)
      return Enum.Parse(type, value);
    if (type == typeof (long))
      return (object) long.Parse(value);
    return type == typeof (ushort) ? (object) ushort.Parse(value) : (object) value;
  }

  private static object TypeConvert(TomlObject obj)
  {
    TomlObjectType tomlType = obj.TomlType;
    switch (tomlType - 1)
    {
      case 0:
        return (object) obj.Get<bool>();
      case 1:
        long num = obj.Get<long>();
        return num >= (long) int.MinValue && num <= (long) int.MaxValue ? (object) obj.Get<int>() : (object) num;
      case 2:
        return (object) obj.Get<float>();
      case 3:
        return (object) obj.Get<string>();
      default:
        throw new InvalidConfigurationException($"Cannot convert {tomlType}.");
    }
  }

  private void InvokeValueChanged(in ConfigurationManager.ValueChangedInvoke invoke)
  {
    try
    {
      Action<CVarChangeInfo> cvarValueChanged = this.OnCVarValueChanged;
      if (cvarValueChanged != null)
        cvarValueChanged(invoke.Info);
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Error while running OnCVarValueChanged callback: {ex}");
    }
    ReadOnlySpan<InvokeList<ConfigurationManager.ValueChangedDelegate>.Entry> entries = invoke.Invoke.Entries;
    for (int index = 0; index < entries.Length; ++index)
    {
      InvokeList<ConfigurationManager.ValueChangedDelegate>.Entry entry = entries[index];
      try
      {
        entry.Value(invoke.Value, in invoke.Info);
      }
      catch (Exception ex)
      {
        this._sawmill.Error($"Error while running OnValueChanged callback: {ex}");
      }
    }
  }

  private ConfigurationManager.ValueChangedInvoke SetupInvokeValueChanged(
    ConfigurationManager.ConfigVar var,
    object newValue,
    object oldValue,
    GameTick? tick = null)
  {
    tick.GetValueOrDefault();
    if (!tick.HasValue)
      tick = new GameTick?(this._gameTiming.CurTick);
    return new ConfigurationManager.ValueChangedInvoke(new CVarChangeInfo(var.Name, tick.Value, newValue, oldValue), var.ValueChanged);
  }

  private IEnumerable<(string cvar, object value)> ParseCVarValuesFromToml(TomlTable tblRoot)
  {
    return ProcessTomlObject((TomlObject) tblRoot, "");

    static IEnumerable<(string cvar, object value)> ProcessTomlObject(
      TomlObject obj,
      string tablePath)
    {
      if (obj is TomlTable tomlTable)
      {
        foreach (KeyValuePair<string, TomlObject> keyValuePair in tomlTable)
        {
          string tablePath1 = !(keyValuePair.Value is TomlTable) ? tablePath + keyValuePair.Key : $"{tablePath}{keyValuePair.Key}.";
          IEnumerator<(string, object)> enumerator = ProcessTomlObject(keyValuePair.Value, tablePath1).GetEnumerator();
          while (enumerator.MoveNext())
            yield return enumerator.Current;
          enumerator = (IEnumerator<(string, object)>) null;
        }
      }
      else
        yield return (tablePath, ConfigurationManager.TypeConvert(obj));
    }
  }

  private static object ConvertToCVarType(object value, Type cVar)
  {
    return cVar.IsEnum ? Enum.Parse(cVar, value.ToString() ?? string.Empty) : Convert.ChangeType(value, cVar);
  }

  internal List<Delegate> GetSubs(string name)
  {
    using (this.Lock.ReadGuard())
    {
      List<Delegate> subs = new List<Delegate>();
      ConfigurationManager.ConfigVar configVar;
      if (!this._configVars.TryGetValue(name, out configVar))
        throw new InvalidConfigurationException($"Trying to get unregistered variable '{name}'");
      ReadOnlySpan<InvokeList<ConfigurationManager.ValueChangedDelegate>.Entry> entries = configVar.ValueChanged.Entries;
      for (int index = 0; index < entries.Length; ++index)
      {
        InvokeList<ConfigurationManager.ValueChangedDelegate>.Entry entry = entries[index];
        subs.Add((Delegate) entry.Equality);
      }
      return subs;
    }
  }

  public void MarkForRollback(params CVarDef[] cVars)
  {
    this.MarkForRollback(((IEnumerable<CVarDef>) cVars).Select<CVarDef, string>((Func<CVarDef, string>) (c => c.Name)).ToArray<string>());
  }

  public void MarkForRollback(params string[] cVars)
  {
    Dictionary<string, object> pending = this.LoadPendingRollbackTable() ?? new Dictionary<string, object>();
    foreach (string cVar in cVars)
      pending[cVar] = this.GetCVar(cVar);
    this.SavePendingRollbackTable(pending);
  }

  public void UnmarkForRollback(params CVarDef[] cVars)
  {
    this.UnmarkForRollback(((IEnumerable<CVarDef>) cVars).Select<CVarDef, string>((Func<CVarDef, string>) (c => c.Name)).ToArray<string>());
  }

  public void UnmarkForRollback(params string[] cVars)
  {
    Dictionary<string, object> pending = this.LoadPendingRollbackTable() ?? new Dictionary<string, object>();
    foreach (string cVar in cVars)
      pending.Remove(cVar);
    this.SavePendingRollbackTable(pending);
  }

  private void SavePendingRollbackTable(Dictionary<string, object> pending)
  {
    string str = Toml.WriteString(this.SaveToTomlTable((IEnumerable<string>) pending.Keys, (Func<string, object>) (cVar => pending[cVar])));
    this.SetCVar<string>(CVars.CfgRollbackData, str, false);
  }

  public void ApplyRollback()
  {
    string cvar = this.GetCVar<string>(CVars.CfgRollbackData);
    if (string.IsNullOrWhiteSpace(cvar))
      return;
    this._sawmill.Debug("We have CVars to roll back!");
    try
    {
      this._sawmill.Info("Rolled back CVars: " + string.Join(", ", (IEnumerable<string>) this.LoadFromTomlTable(Toml.ReadString(cvar))));
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Failed to load rollback data:\n{ex}");
    }
    finally
    {
      this.SetCVar<string>(CVars.CfgRollbackData, "", false);
      this.SaveToFile();
    }
  }

  private Dictionary<string, object>? LoadPendingRollbackTable()
  {
    string cvar = this.GetCVar<string>(CVars.CfgRollbackData);
    if (string.IsNullOrWhiteSpace(cvar))
      return (Dictionary<string, object>) null;
    try
    {
      return this.ParseCVarValuesFromToml(Toml.ReadString(cvar)).ToDictionary<string, object>();
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Failed to load rollback data:\n{ex}");
      return (Dictionary<string, object>) null;
    }
  }

  protected sealed class ConfigVar
  {
    public bool ConfigModified;
    public InvokeList<ConfigurationManager.ValueChangedDelegate> ValueChanged;
    private object _defaultValue;
    private object? _value;
    private object? _overrideValueParsed;

    public ConfigVar(string name, object defaultValue, CVar flags)
    {
      this.Name = name;
      this.Flags = flags;
      this._defaultValue = defaultValue;
    }

    public Type? Type { get; internal set; }

    public string Name { get; }

    public object DefaultValue
    {
      get => this._defaultValue;
      set
      {
        int num = this.Registered ? 1 : 0;
        this._defaultValue = value;
      }
    }

    public CVar Flags { get; set; }

    public object? Value
    {
      get => this._value;
      set
      {
        if (value != null)
        {
          int num = this.Registered ? 1 : 0;
        }
        this._value = value;
      }
    }

    public bool Registered { get; private set; }

    public void Register()
    {
      if (this.Registered)
        return;
      if (this._defaultValue == null)
        throw new NullReferenceException("Must specify default value before registering");
      if (this.Value != null && this.DefaultValue.GetType() != this.Value.GetType())
        throw new Exception("The cvar value & default value must be of the same type");
      this.Type = this.OverrideValueParsed == null || !(this.DefaultValue.GetType() != this.OverrideValueParsed.GetType()) ? this.DefaultValue.GetType() : throw new Exception("The cvar override value & default value must be of the same type");
      this.Registered = true;
    }

    public string? OverrideValue { get; set; }

    public object? OverrideValueParsed
    {
      get => this._overrideValueParsed;
      set
      {
        if (value != null)
        {
          int num = this.Registered ? 1 : 0;
        }
        this._overrideValueParsed = value;
      }
    }
  }

  private struct ValueChangedInvoke
  {
    public InvokeList<ConfigurationManager.ValueChangedDelegate> Invoke;
    public CVarChangeInfo Info;

    public object Value => this.Info.NewValue;

    public ValueChangedInvoke(
      CVarChangeInfo info,
      InvokeList<ConfigurationManager.ValueChangedDelegate> invoke)
      : this()
    {
      this.Info = info;
      this.Invoke = invoke;
    }
  }

  protected delegate void ValueChangedDelegate(object value, in CVarChangeInfo info);

  private abstract class ConfigFileStorage
  {
  }

  private sealed class ConfigFileStorageDisk : ConfigurationManager.ConfigFileStorage
  {
    public required string Path;

    public override string ToString() => this.Path;
  }

  private sealed class ConfigFileStorageVirtual : ConfigurationManager.ConfigFileStorage
  {
    public readonly MemoryStream Stream = new MemoryStream();

    public override string ToString() => "<VIRTUAL>";
  }
}
