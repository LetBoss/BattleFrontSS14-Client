// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.ILocalizationManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Localization;

[NotContentImplementable]
public interface ILocalizationManager
{
  string GetString(string messageId);

  bool HasString(string messageId);

  bool TryGetString(string messageId, [NotNullWhen(true)] out string? value);

  string GetString(string messageId, params (string, object)[] args);

  string GetString(string messageId, (string, object) arg);

  string GetString(string messageId, (string, object) arg, (string, object) arg2);

  bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, (string, object) arg);

  bool TryGetString(
    string messageId,
    [NotNullWhen(true)] out string? value,
    (string, object) arg1,
    (string, object) arg2);

  bool TryGetString(string messageId, [NotNullWhen(true)] out string? value, params (string, object)[] keyArgs);

  CultureInfo? DefaultCulture { get; set; }

  void SetCulture(CultureInfo culture);

  bool HasCulture(CultureInfo culture);

  void LoadCulture(CultureInfo culture);

  CultureInfo SetDefaultCulture();

  List<CultureInfo> GetFoundCultures();

  void SetFallbackCluture(params CultureInfo[] culture);

  void ReloadLocalizations();

  void AddFunction(CultureInfo culture, string name, LocFunction function);

  EntityLocData GetEntityData(string prototypeId);

  void Initialize()
  {
  }
}
