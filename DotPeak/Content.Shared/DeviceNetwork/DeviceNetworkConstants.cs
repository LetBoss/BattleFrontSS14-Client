// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.DeviceNetworkConstants
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.DeviceNetwork;

public static class DeviceNetworkConstants
{
  public const string LogicState = "logic_state";
  public const string Command = "command";
  public const string CmdSetState = "set_state";
  public const string CmdUpdatedState = "updated_state";
  public const string StateEnabled = "state_enabled";

  public static string FrequencyToString(this uint frequency)
  {
    string str = frequency.ToString();
    return str.Length <= 2 ? str + ".0" : str.Insert(str.Length - 1, ".");
  }

  public static string DeviceNetIdToLocalizedName(this int id)
  {
    if (!Enum.IsDefined(typeof (DeviceNetworkComponent.DeviceNetIdDefaults), (object) id))
      return id.ToString();
    string str1 = ((DeviceNetworkComponent.DeviceNetIdDefaults) id).ToString();
    string str2 = "device-net-id-" + CaseConversion.PascalToKebab(str1);
    string str3;
    return IoCManager.Resolve<ILocalizationManager>().TryGetString(str2, ref str3) ? str3 : str1;
  }
}
