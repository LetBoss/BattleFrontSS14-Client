// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.NetworkConfiguratorLinksSaveMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.DeviceNetwork;

[NetSerializable]
[Serializable]
public sealed class NetworkConfiguratorLinksSaveMessage : BoundUserInterfaceMessage
{
  public readonly List<(string source, string sink)> Links;

  public NetworkConfiguratorLinksSaveMessage(List<(string source, string sink)> links)
  {
    this.Links = links;
  }
}
