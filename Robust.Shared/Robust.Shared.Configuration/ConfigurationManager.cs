using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Nett;
using Robust.Shared.Analyzers;
using Robust.Shared.Collections;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.Configuration;

[Virtual]
internal class ConfigurationManager : IConfigurationManagerInternal, IConfigurationManager
{
	protected sealed class ConfigVar
	{
		public bool ConfigModified;

		public InvokeList<ValueChangedDelegate> ValueChanged;

		private object _defaultValue;

		private object? _value;

		private object? _overrideValueParsed;

		public Type? Type { get; internal set; }

		public string Name { get; }

		public object DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				_ = Registered;
				_defaultValue = value;
			}
		}

		public CVar Flags { get; set; }

		public object? Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (value != null)
				{
					_ = Registered;
				}
				_value = value;
			}
		}

		public bool Registered { get; private set; }

		public string? OverrideValue { get; set; }

		public object? OverrideValueParsed
		{
			get
			{
				return _overrideValueParsed;
			}
			set
			{
				if (value != null)
				{
					_ = Registered;
				}
				_overrideValueParsed = value;
			}
		}

		public ConfigVar(string name, object defaultValue, CVar flags)
		{
			Name = name;
			Flags = flags;
			_defaultValue = defaultValue;
		}

		public void Register()
		{
			if (!Registered)
			{
				if (_defaultValue == null)
				{
					throw new NullReferenceException("Must specify default value before registering");
				}
				if (Value != null && DefaultValue.GetType() != Value.GetType())
				{
					throw new Exception("The cvar value & default value must be of the same type");
				}
				if (OverrideValueParsed != null && DefaultValue.GetType() != OverrideValueParsed.GetType())
				{
					throw new Exception("The cvar override value & default value must be of the same type");
				}
				Type = DefaultValue.GetType();
				Registered = true;
			}
		}
	}

	private struct ValueChangedInvoke
	{
		public InvokeList<ValueChangedDelegate> Invoke;

		public CVarChangeInfo Info;

		public object Value => Info.NewValue;

		public ValueChangedInvoke(CVarChangeInfo info, InvokeList<ValueChangedDelegate> invoke)
		{
			this = default(ValueChangedInvoke);
			Info = info;
			Invoke = invoke;
		}
	}

	protected delegate void ValueChangedDelegate(object value, in CVarChangeInfo info);

	private abstract class ConfigFileStorage
	{
	}

	private sealed class ConfigFileStorageDisk : ConfigFileStorage
	{
		public required string Path;

		public override string ToString()
		{
			return Path;
		}
	}

	private sealed class ConfigFileStorageVirtual : ConfigFileStorage
	{
		public readonly MemoryStream Stream = new MemoryStream();

		public override string ToString()
		{
			return "<VIRTUAL>";
		}
	}

	[Robust.Shared.IoC.Dependency]
	private readonly IGameTiming _gameTiming;

	[Robust.Shared.IoC.Dependency]
	private readonly ILogManager _logManager;

	private const char TABLE_DELIMITER = '.';

	protected readonly Dictionary<string, ConfigVar> _configVars = new Dictionary<string, ConfigVar>();

	private ConfigFileStorage? _configFile;

	protected bool _isServer;

	protected readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

	private ISawmill _sawmill;

	public event Action<CVarChangeInfo>? OnCVarValueChanged;

	public void Initialize(bool isServer)
	{
		_isServer = isServer;
		_sawmill = _logManager.GetSawmill("cfg");
	}

	public virtual void Shutdown()
	{
		using (Lock.WriteGuard())
		{
			_configVars.Clear();
			_configFile = null;
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
			_sawmill.Error("Unable to load configuration from table:\n{0}", ex);
			return new HashSet<string>();
		}
		return LoadFromTomlTable(table);
	}

	private HashSet<string> LoadFromTomlTable(TomlTable table)
	{
		HashSet<string> hashSet = new HashSet<string>();
		ValueList<ValueChangedInvoke> callbackEvents = default(ValueList<ValueChangedInvoke>);
		try
		{
			using (Lock.WriteGuard())
			{
				foreach (var (text, value) in ParseCVarValuesFromToml(table))
				{
					hashSet.Add(text);
					LoadParsedVar(text, value, ref callbackEvents);
				}
				return hashSet;
			}
		}
		finally
		{
			RunDeferredInvokeCallbacks(in callbackEvents);
		}
	}

	private void RunDeferredInvokeCallbacks(in ValueList<ValueChangedInvoke> callbackEvents)
	{
		foreach (ValueChangedInvoke callbackEvent in callbackEvents)
		{
			InvokeValueChanged(callbackEvent);
		}
	}

	private void LoadParsedVar(string cvar, object value, ref ValueList<ValueChangedInvoke> changedInvokes)
	{
		if (_configVars.TryGetValue(cvar, out ConfigVar value2))
		{
			object configVarValue = GetConfigVarValue(value2);
			object obj = value;
			if (value2.Type != value.GetType())
			{
				try
				{
					obj = ConvertToCVarType(value, value2.Type);
				}
				catch
				{
					_sawmill.Error($"Parsed cvar does not match registered cvar type. Name: {cvar}. Code Type: {value2.Type}. Parsed type: {value.GetType()}");
					return;
				}
			}
			changedInvokes.Add(SetupInvokeValueChanged(value2, obj, configVarValue));
			value2.Value = obj;
		}
		else
		{
			value2 = AddUnregisteredCVar(cvar, value);
		}
		value2.ConfigModified = true;
	}

	private ConfigVar AddUnregisteredCVar(string name, object value)
	{
		ConfigVar configVar = new ConfigVar(name, null, CVar.NONE)
		{
			Value = value
		};
		_configVars.Add(name, configVar);
		return configVar;
	}

	public HashSet<string> LoadDefaultsFromTomlStream(Stream stream)
	{
		TomlTable tblRoot = Toml.ReadStream(stream);
		HashSet<string> result = new HashSet<string>();
		ValueList<ValueChangedInvoke> valueList = default(ValueList<ValueChangedInvoke>);
		using (Lock.WriteGuard())
		{
			foreach (var (text, obj) in ParseCVarValuesFromToml(tblRoot))
			{
				if (!_configVars.TryGetValue(text, out ConfigVar value) || !value.Registered)
				{
					_sawmill.Error("Trying to set unregistered variable '" + text + "'");
					continue;
				}
				object obj2 = obj;
				if (value.Type != obj.GetType())
				{
					try
					{
						obj2 = ConvertToCVarType(obj, value.Type);
					}
					catch
					{
						_sawmill.Error($"Override TOML parsed cvar does not match registered cvar type. Name: {text}. Code Type: {value.Type}. Toml type: {obj.GetType()}");
						continue;
					}
				}
				if (value.OverrideValue == null && value.Value == null)
				{
					object configVarValue = GetConfigVarValue(value);
					valueList.Add(SetupInvokeValueChanged(value, obj2, configVarValue));
				}
				value.DefaultValue = obj2;
			}
		}
		foreach (ValueChangedInvoke item in valueList)
		{
			InvokeValueChanged(item);
		}
		return result;
	}

	public HashSet<string> LoadFromFile(string configFile)
	{
		try
		{
			HashSet<string> result;
			using (FileStream file = File.OpenRead(configFile))
			{
				result = LoadFromTomlStream(file);
			}
			SetSaveFile(configFile);
			ApplyRollback();
			_sawmill.Info("Configuration loaded from file");
			return result;
		}
		catch (Exception ex)
		{
			_sawmill.Error("Unable to load configuration file:\n{0}", ex);
			return new HashSet<string>(0);
		}
	}

	public void SetSaveFile(string configFile)
	{
		_configFile = new ConfigFileStorageDisk
		{
			Path = configFile
		};
	}

	public void SetVirtualConfig()
	{
		_configFile = new ConfigFileStorageVirtual();
	}

	public void CheckUnusedCVars()
	{
		if (!GetCVar(CVars.CfgCheckUnused))
		{
			return;
		}
		using (Lock.ReadGuard())
		{
			foreach (ConfigVar value in _configVars.Values)
			{
				if (!value.Registered)
				{
					_sawmill.Warning("Unknown CVar found (typo in config?): {CVar}", value.Name);
				}
			}
		}
	}

	public void SaveToTomlStream(Stream stream, IEnumerable<string> cvars)
	{
		Toml.WriteStream(SaveToTomlTable(cvars), stream);
	}

	private TomlTable SaveToTomlTable(IEnumerable<string> cvars, Func<string, object>? overrideValue = null)
	{
		TomlTable val = Toml.Create();
		using (Lock.ReadGuard())
		{
			Unsafe.SkipInit(out TomlObject added);
			foreach (string cvar in cvars)
			{
				if (!_configVars.TryGetValue(cvar, out ConfigVar value))
				{
					continue;
				}
				object obj;
				if (overrideValue != null)
				{
					obj = overrideValue(cvar);
				}
				else
				{
					obj = value.Value;
					if (obj == null && value.Registered)
					{
						obj = value.DefaultValue;
					}
				}
				if (obj == null)
				{
					_sawmill.Error("CVar " + cvar + " has no value or default value, was the default value registered as null?");
					continue;
				}
				int num = cvar.LastIndexOf('.');
				string[] array = cvar.Substring(0, num).Split('.');
				string text = cvar.Substring(num + 1);
				TomlTable val2 = val;
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (!val2.TryGetValue(text2, ref added))
					{
						added = (TomlObject)(object)TomlObjectFactory.Add<TomlObject>(val2, text2, (IDictionary<string, TomlObject>)new Dictionary<string, TomlObject>(), (TableTypes)0).Added;
					}
					val2 = (TomlTable)(object)(((added is TomlTable) ? added : null) ?? throw new InvalidConfigurationException($"[CFG] Object {text2} is being used like a table, but it is a {added}. Are your CVar names formed properly?"));
				}
				if (!(obj is Enum obj2))
				{
					if (!(obj is int num2))
					{
						if (!(obj is long num3))
						{
							if (!(obj is bool flag))
							{
								if (!(obj is string text3))
								{
									if (!(obj is float num4))
									{
										if (obj is double num5)
										{
											TomlObjectFactory.Add(val2, text, num5);
										}
										else
										{
											_sawmill.Warning("Cannot serialize '" + cvar + "', unsupported type.");
										}
									}
									else
									{
										TomlObjectFactory.Add(val2, text, num4);
									}
								}
								else
								{
									TomlObjectFactory.Add(val2, text, text3);
								}
							}
							else
							{
								TomlObjectFactory.Add(val2, text, flag);
							}
						}
						else
						{
							TomlObjectFactory.Add(val2, text, num3);
						}
					}
					else
					{
						TomlObjectFactory.Add(val2, text, num2);
					}
				}
				else
				{
					TomlObjectFactory.Add(val2, text, (int)(object)obj2);
				}
			}
			return val;
		}
	}

	public void SaveToFile()
	{
		if (_configFile == null)
		{
			_sawmill.Warning("Cannot save the config file, because one was never loaded.");
			return;
		}
		try
		{
			IEnumerable<string> cvars = from x in _configVars
				where x.Value.ConfigModified || ((x.Value.Flags & CVar.ARCHIVE) != CVar.NONE && x.Value.Value != null && !x.Value.Value.Equals(x.Value.DefaultValue))
				select x.Key;
			MemoryStream memoryStream = new MemoryStream();
			SaveToTomlStream(memoryStream, cvars);
			memoryStream.Position = 0L;
			ConfigFileStorage configFile = _configFile;
			if (!(configFile is ConfigFileStorageDisk configFileStorageDisk))
			{
				if (!(configFile is ConfigFileStorageVirtual configFileStorageVirtual))
				{
					throw new UnreachableException();
				}
				configFileStorageVirtual.Stream.SetLength(0L);
				memoryStream.CopyTo(configFileStorageVirtual.Stream);
			}
			else
			{
				using FileStream destination = File.Create(configFileStorageDisk.Path);
				memoryStream.CopyTo(destination);
			}
			_sawmill.Info($"config saved to '{_configFile}'.");
		}
		catch (Exception value)
		{
			_sawmill.Warning($"Cannot save the config file '{_configFile}'.\n {value}");
		}
	}

	public void RegisterCVar<T>(string name, T defaultValue, CVar flags = CVar.NONE, Action<T>? onValueChanged = null) where T : notnull
	{
		RegisterCVar(name, typeof(T), defaultValue, flags);
		if (onValueChanged != null)
		{
			OnValueChanged(name, onValueChanged);
		}
	}

	private void RegisterCVar(string name, Type type, object defaultValue, CVar flags)
	{
		CVar cVar = (_isServer ? CVar.CLIENTONLY : CVar.SERVERONLY);
		if ((flags & cVar) != CVar.NONE)
		{
			return;
		}
		using (Lock.WriteGuard())
		{
			if (_configVars.TryGetValue(name, out ConfigVar value))
			{
				if (value.Registered)
				{
					_sawmill.Error("The variable '" + name + "' has already been registered.");
				}
				if (value.Value != null && type != value.Value.GetType())
				{
					try
					{
						value.Value = ConvertToCVarType(value.Value, type);
					}
					catch
					{
						_sawmill.Error($"TOML parsed cvar does not match registered cvar type. Name: {name}. Code Type: {type.Name}. Toml type: {value.Value.GetType().Name}");
						return;
					}
				}
				value.DefaultValue = defaultValue;
				value.Flags = flags;
				value.Register();
				if (value.OverrideValue != null)
				{
					value.OverrideValueParsed = ParseOverrideValue(value.OverrideValue, type);
				}
			}
			else
			{
				ConfigVar configVar = new ConfigVar(name, defaultValue, flags);
				configVar.Register();
				_configVars.Add(name, configVar);
			}
		}
	}

	public void OnValueChanged<T>(CVarDef<T> cVar, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		OnValueChanged(cVar.Name, onValueChanged, invokeImmediately);
	}

	public void OnValueChanged<T>(string name, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		using (Lock.WriteGuard())
		{
			_configVars[name].ValueChanged.AddInPlace(delegate(object value, in CVarChangeInfo _)
			{
				onValueChanged((T)value);
			}, onValueChanged);
		}
		if (invokeImmediately)
		{
			onValueChanged(GetCVar<T>(name));
		}
	}

	public void UnsubValueChanged<T>(CVarDef<T> cVar, Action<T> onValueChanged) where T : notnull
	{
		UnsubValueChanged(cVar.Name, onValueChanged);
	}

	public void UnsubValueChanged<T>(string name, Action<T> onValueChanged) where T : notnull
	{
		using (Lock.WriteGuard())
		{
			_configVars[name].ValueChanged.RemoveInPlace(onValueChanged);
		}
	}

	public void OnValueChanged<T>(CVarDef<T> cVar, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		OnValueChanged(cVar.Name, onValueChanged, invokeImmediately);
	}

	public void OnValueChanged<T>(string name, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		object configVarValue;
		using (Lock.WriteGuard())
		{
			ConfigVar configVar = _configVars[name];
			configVarValue = GetConfigVarValue(configVar);
			configVar.ValueChanged.AddInPlace(delegate(object value, in CVarChangeInfo info)
			{
				onValueChanged((T)value, in info);
			}, onValueChanged);
		}
		if (invokeImmediately)
		{
			onValueChanged(GetCVar<T>(name), new CVarChangeInfo(name, _gameTiming.CurTick, configVarValue, configVarValue));
		}
	}

	public void UnsubValueChanged<T>(CVarDef<T> cVar, CVarChanged<T> onValueChanged) where T : notnull
	{
		UnsubValueChanged(cVar.Name, onValueChanged);
	}

	public void UnsubValueChanged<T>(string name, CVarChanged<T> onValueChanged) where T : notnull
	{
		using (Lock.WriteGuard())
		{
			_configVars[name].ValueChanged.RemoveInPlace(onValueChanged);
		}
	}

	public void LoadCVarsFromAssembly(Assembly assembly)
	{
		foreach (Type item in from p in assembly.GetTypes()
			where Attribute.IsDefined(p, typeof(CVarDefsAttribute))
			select p)
		{
			LoadCVarsFromType(item);
		}
	}

	public void LoadCVarsFromType(Type containingType)
	{
		FieldInfo[] fields = containingType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			Type fieldType = fieldInfo.FieldType;
			if (fieldType.IsGenericType && !(fieldType.GetGenericTypeDefinition() != typeof(CVarDef<>)))
			{
				Type type = fieldType.GetGenericArguments()[0];
				if (!fieldInfo.IsInitOnly)
				{
					throw new InvalidOperationException($"Found CVarDef '{fieldInfo.Name}' on '{fieldInfo.DeclaringType?.FullName}' that is not readonly. Please mark it as readonly.");
				}
				CVarDef cVarDef = (CVarDef)fieldInfo.GetValue(null);
				if (cVarDef == null)
				{
					throw new InvalidOperationException($"CVarDef '{fieldInfo.Name}' on '{fieldInfo.DeclaringType?.FullName}' is null.");
				}
				RegisterCVar(cVarDef.Name, type, cVarDef.DefaultValue, cVarDef.Flags);
			}
		}
	}

	public bool IsCVarRegistered(string name)
	{
		using (Lock.ReadGuard())
		{
			ConfigVar value;
			return _configVars.TryGetValue(name, out value) && value.Registered;
		}
	}

	public CVar GetCVarFlags(string name)
	{
		using (Lock.ReadGuard())
		{
			return _configVars[name].Flags;
		}
	}

	public IEnumerable<string> GetRegisteredCVars()
	{
		using (Lock.ReadGuard())
		{
			return (from p in _configVars
				where p.Value.Registered
				select p.Key).ToArray();
		}
	}

	public virtual void SetCVar(string name, object value, bool force = false)
	{
		SetCVarInternal(name, value, _gameTiming.CurTick);
	}

	protected void SetCVarInternal(string name, object value, GameTick intendedTick)
	{
		ValueChangedInvoke? valueChangedInvoke = null;
		using (Lock.WriteGuard())
		{
			if (!_configVars.TryGetValue(name, out ConfigVar value2) || !value2.Registered)
			{
				throw new InvalidConfigurationException("Trying to set unregistered variable '" + name + "'");
			}
			if (!object.Equals(value2.OverrideValueParsed ?? value2.Value, value))
			{
				object configVarValue = GetConfigVarValue(value2);
				valueChangedInvoke = SetupInvokeValueChanged(value2, value, configVarValue, intendedTick);
				value2.OverrideValue = null;
				value2.OverrideValueParsed = null;
				value2.Value = value;
			}
		}
		if (valueChangedInvoke.HasValue)
		{
			InvokeValueChanged(valueChangedInvoke.Value);
		}
	}

	public void SetCVar<T>(CVarDef<T> def, T value, bool force = false) where T : notnull
	{
		SetCVar(def.Name, value, force);
	}

	public void OverrideDefault(string name, object value)
	{
		ValueChangedInvoke? valueChangedInvoke = null;
		using (Lock.WriteGuard())
		{
			if (!_configVars.TryGetValue(name, out ConfigVar value2) || !value2.Registered)
			{
				throw new InvalidConfigurationException("Trying to set unregistered variable '" + name + "'");
			}
			if (value2.OverrideValue == null && value2.Value == null)
			{
				object configVarValue = GetConfigVarValue(value2);
				valueChangedInvoke = SetupInvokeValueChanged(value2, value, configVarValue);
			}
			value2.DefaultValue = value;
		}
		if (valueChangedInvoke.HasValue)
		{
			InvokeValueChanged(valueChangedInvoke.Value);
		}
	}

	public void OverrideDefault<T>(CVarDef<T> def, T value) where T : notnull
	{
		OverrideDefault(def.Name, value);
	}

	public object GetCVar(string name)
	{
		using (Lock.ReadGuard())
		{
			if (_configVars.TryGetValue(name, out ConfigVar value) && value.Registered)
			{
				return GetConfigVarValue(value);
			}
			throw new InvalidConfigurationException("Trying to get unregistered variable '" + name + "'");
		}
	}

	public T GetCVar<T>(string name)
	{
		return (T)GetCVar(name);
	}

	public T GetCVar<T>(CVarDef<T> def) where T : notnull
	{
		return GetCVar<T>(def.Name);
	}

	public Type GetCVarType(string name)
	{
		using (Lock.ReadGuard())
		{
			if (!_configVars.TryGetValue(name, out ConfigVar value) || !value.Registered)
			{
				throw new InvalidConfigurationException("Trying to get type of unregistered variable '" + name + "'");
			}
			return value.Type;
		}
	}

	protected static object GetConfigVarValue(ConfigVar cVar)
	{
		return cVar.OverrideValueParsed ?? cVar.Value ?? cVar.DefaultValue;
	}

	public void OverrideConVars(IEnumerable<(string key, string value)> cVars)
	{
		ValueList<ValueChangedInvoke> valueList = default(ValueList<ValueChangedInvoke>);
		using (Lock.WriteGuard())
		{
			foreach (var (text, text2) in cVars)
			{
				if (_configVars.TryGetValue(text, out ConfigVar value))
				{
					value.OverrideValue = text2;
					if (value.Registered)
					{
						object obj = ParseOverrideValue(text2, value.Type);
						object configVarValue = GetConfigVarValue(value);
						valueList.Add(SetupInvokeValueChanged(value, obj, configVarValue));
						value.OverrideValueParsed = obj;
					}
				}
				else
				{
					ConfigVar value2 = new ConfigVar(text, null, CVar.NONE)
					{
						OverrideValue = text2
					};
					_configVars.Add(text, value2);
				}
			}
		}
		foreach (ValueChangedInvoke item in valueList)
		{
			InvokeValueChanged(item);
		}
	}

	private static object ParseOverrideValue(string value, Type? type)
	{
		if (type == typeof(int))
		{
			return int.Parse(value);
		}
		if (type == typeof(bool))
		{
			return bool.Parse(value);
		}
		if (type == typeof(float))
		{
			return float.Parse(value, CultureInfo.InvariantCulture);
		}
		if ((object)type != null && type.IsEnum)
		{
			return Enum.Parse(type, value);
		}
		if (type == typeof(long))
		{
			return long.Parse(value);
		}
		if (type == typeof(ushort))
		{
			return ushort.Parse(value);
		}
		return value;
	}

	private static object TypeConvert(TomlObject obj)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected I4, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		TomlObjectType tomlType = obj.TomlType;
		switch (tomlType - 1)
		{
		case 0:
			return obj.Get<bool>();
		case 2:
			return obj.Get<float>();
		case 1:
		{
			long num = obj.Get<long>();
			if (num >= int.MinValue && num <= int.MaxValue)
			{
				return obj.Get<int>();
			}
			return num;
		}
		case 3:
			return obj.Get<string>();
		default:
			throw new InvalidConfigurationException($"Cannot convert {tomlType}.");
		}
	}

	private void InvokeValueChanged(in ValueChangedInvoke invoke)
	{
		try
		{
			this.OnCVarValueChanged?.Invoke(invoke.Info);
		}
		catch (Exception value)
		{
			_sawmill.Error($"Error while running OnCVarValueChanged callback: {value}");
		}
		ReadOnlySpan<InvokeList<ValueChangedDelegate>.Entry> entries = invoke.Invoke.Entries;
		for (int i = 0; i < entries.Length; i++)
		{
			InvokeList<ValueChangedDelegate>.Entry entry = entries[i];
			try
			{
				entry.Value(invoke.Value, in invoke.Info);
			}
			catch (Exception value2)
			{
				_sawmill.Error($"Error while running OnValueChanged callback: {value2}");
			}
		}
	}

	private ValueChangedInvoke SetupInvokeValueChanged(ConfigVar var, object newValue, object oldValue, GameTick? tick = null)
	{
		GameTick valueOrDefault = tick.GetValueOrDefault();
		if (!tick.HasValue)
		{
			valueOrDefault = _gameTiming.CurTick;
			tick = valueOrDefault;
		}
		return new ValueChangedInvoke(new CVarChangeInfo(var.Name, tick.Value, newValue, oldValue), var.ValueChanged);
	}

	private IEnumerable<(string cvar, object value)> ParseCVarValuesFromToml(TomlTable tblRoot)
	{
		return ProcessTomlObject((TomlObject)(object)tblRoot, "");
		static IEnumerable<(string cvar, object value)> ProcessTomlObject(TomlObject obj, string tablePath)
		{
			TomlTable val = (TomlTable)(object)((obj is TomlTable) ? obj : null);
			if (val != null)
			{
				foreach (KeyValuePair<string, TomlObject> item2 in val)
				{
					foreach (var item3 in ProcessTomlObject(tablePath: (!(item2.Value is TomlTable)) ? (tablePath + item2.Key) : (tablePath + item2.Key + "."), obj: item2.Value))
					{
						yield return item3;
					}
				}
			}
			else
			{
				object item = TypeConvert(obj);
				yield return (cvar: tablePath, value: item);
			}
		}
	}

	private static object ConvertToCVarType(object value, Type cVar)
	{
		if (cVar.IsEnum)
		{
			return Enum.Parse(cVar, value.ToString() ?? string.Empty);
		}
		return Convert.ChangeType(value, cVar);
	}

	internal List<Delegate> GetSubs(string name)
	{
		using (Lock.ReadGuard())
		{
			List<Delegate> list = new List<Delegate>();
			if (!_configVars.TryGetValue(name, out ConfigVar value))
			{
				throw new InvalidConfigurationException("Trying to get unregistered variable '" + name + "'");
			}
			ReadOnlySpan<InvokeList<ValueChangedDelegate>.Entry> entries = value.ValueChanged.Entries;
			for (int i = 0; i < entries.Length; i++)
			{
				InvokeList<ValueChangedDelegate>.Entry entry = entries[i];
				list.Add((Delegate)entry.Equality);
			}
			return list;
		}
	}

	public void MarkForRollback(params CVarDef[] cVars)
	{
		MarkForRollback(cVars.Select((CVarDef c) => c.Name).ToArray());
	}

	public void MarkForRollback(params string[] cVars)
	{
		Dictionary<string, object> dictionary = LoadPendingRollbackTable() ?? new Dictionary<string, object>();
		foreach (string text in cVars)
		{
			dictionary[text] = GetCVar(text);
		}
		SavePendingRollbackTable(dictionary);
	}

	public void UnmarkForRollback(params CVarDef[] cVars)
	{
		UnmarkForRollback(cVars.Select((CVarDef c) => c.Name).ToArray());
	}

	public void UnmarkForRollback(params string[] cVars)
	{
		Dictionary<string, object> dictionary = LoadPendingRollbackTable() ?? new Dictionary<string, object>();
		foreach (string key in cVars)
		{
			dictionary.Remove(key);
		}
		SavePendingRollbackTable(dictionary);
	}

	private void SavePendingRollbackTable(Dictionary<string, object> pending)
	{
		string value = Toml.WriteString(SaveToTomlTable(pending.Keys, (string cVar) => pending[cVar]));
		SetCVar(CVars.CfgRollbackData, value);
	}

	public void ApplyRollback()
	{
		string cVar = GetCVar(CVars.CfgRollbackData);
		if (string.IsNullOrWhiteSpace(cVar))
		{
			return;
		}
		_sawmill.Debug("We have CVars to roll back!");
		try
		{
			TomlTable table = Toml.ReadString(cVar);
			HashSet<string> values = LoadFromTomlTable(table);
			_sawmill.Info("Rolled back CVars: " + string.Join(", ", values));
		}
		catch (Exception value)
		{
			_sawmill.Error($"Failed to load rollback data:\n{value}");
		}
		finally
		{
			SetCVar(CVars.CfgRollbackData, "");
			SaveToFile();
		}
	}

	private Dictionary<string, object>? LoadPendingRollbackTable()
	{
		string cVar = GetCVar(CVars.CfgRollbackData);
		if (string.IsNullOrWhiteSpace(cVar))
		{
			return null;
		}
		try
		{
			TomlTable tblRoot = Toml.ReadString(cVar);
			return ParseCVarValuesFromToml(tblRoot).ToDictionary();
		}
		catch (Exception value)
		{
			_sawmill.Error($"Failed to load rollback data:\n{value}");
			return null;
		}
	}
}
