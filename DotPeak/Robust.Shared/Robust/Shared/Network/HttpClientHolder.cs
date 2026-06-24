// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.HttpClientHolder
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System.Net.Http;

#nullable enable
namespace Robust.Shared.Network;

internal sealed class HttpClientHolder : IHttpClientHolder
{
  public HttpClient Client { get; }

  public HttpClientHolder()
  {
    this.Client = new HttpClient((HttpMessageHandler) HappyEyeballsHttp.CreateHttpHandler());
    HttpClientUserAgent.AddUserAgent(this.Client);
  }
}
