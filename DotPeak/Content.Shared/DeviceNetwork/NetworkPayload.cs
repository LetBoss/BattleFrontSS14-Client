// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.NetworkPayload
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.DeviceNetwork;

public sealed class NetworkPayload : Dictionary<string, object?>
{
  public bool TryGetValue<T>(string key, [NotNullWhen(true)] out T? value)
  {
    T obj;
    if (Extensions.TryCastValue<T, string, object>((Dictionary<string, object>) this, key, ref obj))
    {
      value = obj;
      return true;
    }
    value = default (T);
    return false;
  }
}
