// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.IConfigurationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace Robust.Shared.Configuration;

[NotContentImplementable]
public interface IConfigurationManager
{
  void SaveToFile();

  void SaveToTomlStream(Stream stream, IEnumerable<string> cvars);

  HashSet<string> LoadFromTomlStream(Stream stream);

  HashSet<string> LoadDefaultsFromTomlStream(Stream stream);

  void RegisterCVar<T>(string name, T defaultValue, CVar flags = CVar.NONE, Action<T>? onValueChanged = null) where T : notnull;

  bool IsCVarRegistered(string name);

  CVar GetCVarFlags(string name);

  IEnumerable<string> GetRegisteredCVars();

  void SetCVar(string name, object value, bool force = false);

  void SetCVar<T>(CVarDef<T> def, T value, bool force = false) where T : notnull;

  void OverrideDefault(string name, object value);

  void OverrideDefault<T>(CVarDef<T> def, T value) where T : notnull;

  object GetCVar(string name);

  T GetCVar<T>(string name);

  T GetCVar<T>(CVarDef<T> def) where T : notnull;

  Type GetCVarType(string name);

  void OnValueChanged<T>(CVarDef<T> cVar, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull;

  void OnValueChanged<T>(string name, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull;

  void UnsubValueChanged<T>(CVarDef<T> cVar, Action<T> onValueChanged) where T : notnull;

  void UnsubValueChanged<T>(string name, Action<T> onValueChanged) where T : notnull;

  void OnValueChanged<T>(CVarDef<T> cVar, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull;

  void OnValueChanged<T>(string name, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull;

  void UnsubValueChanged<T>(CVarDef<T> cVar, CVarChanged<T> onValueChanged) where T : notnull;

  void UnsubValueChanged<T>(string name, CVarChanged<T> onValueChanged) where T : notnull;

  event Action<CVarChangeInfo>? OnCVarValueChanged;

  void MarkForRollback(params CVarDef[] cVars);

  void MarkForRollback(params string[] cVars);

  void UnmarkForRollback(params CVarDef[] cVars);

  void UnmarkForRollback(params string[] cVars);

  void ApplyRollback();
}
