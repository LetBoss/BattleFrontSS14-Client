// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.Loc
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Localization;

public static class Loc
{
  private static ILocalizationManager LocalizationManager
  {
    get => IoCManager.Resolve<ILocalizationManager>();
  }

  public static string GetString(string messageId) => Loc.LocalizationManager.GetString(messageId);

  [Obsolete("Use ILocalizationManager")]
  public static bool TryGetString(string messageId, [NotNullWhen(true)] out string? message)
  {
    return Loc.LocalizationManager.TryGetString(messageId, out message);
  }

  public static string GetString(string messageId, params (string, object)[] args)
  {
    return Loc.LocalizationManager.GetString(messageId, args);
  }

  [Obsolete("Use ILocalizationManager")]
  public static bool TryGetString(
    string messageId,
    [NotNullWhen(true)] out string? value,
    params (string, object)[] args)
  {
    return Loc.LocalizationManager.TryGetString(messageId, out value, args);
  }
}
