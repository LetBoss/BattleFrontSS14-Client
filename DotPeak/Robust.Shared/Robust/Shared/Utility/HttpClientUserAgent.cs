// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.HttpClientUserAgent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Net.Http;
using System.Net.Http.Headers;

#nullable enable
namespace Robust.Shared.Utility;

internal static class HttpClientUserAgent
{
  private const string ProductName = "RobustToolbox";

  public static void AddUserAgent(HttpClient client)
  {
    Version version = typeof (HttpClientUserAgent).Assembly.GetName().Version;
    if ((object) version == null)
      return;
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RobustToolbox", version.ToString()));
  }
}
