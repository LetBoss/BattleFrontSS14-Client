// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.StringTable
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Network;

internal sealed class StringTable
{
  private const int StringTablePacketId = 0;
  private bool _initialized;
  private readonly INetManager _network;
  private readonly Dictionary<int, string> _strings;
  private int _lastStringIndex;
  private InitCallback? _callback;
  private StringTableUpdateCallback? _updateCallback;
  public ISawmill Sawmill;

  internal Dictionary<int, string> Strings => this._strings;

  public StringTable(INetManager network)
  {
    this._network = network;
    this._strings = new Dictionary<int, string>();
  }

  public static int InvalidStringId => -1;

  public void Initialize(InitCallback? callback = null, StringTableUpdateCallback? updateCallback = null)
  {
    this._callback = callback;
    this._updateCallback = updateCallback;
    this._network.RegisterNetMessage<MsgStringTableEntries>(new ProcessMessage<MsgStringTableEntries>(this.ReceiveEntries), NetMessageAccept.Client | NetMessageAccept.Handshake);
    this.Reset();
    this._initialized = true;
  }

  private void ReceiveEntries(MsgStringTableEntries message)
  {
    this.Sawmill.Info("Received message name string table.");
    foreach (MsgStringTableEntries.Entry entry in message.Entries)
    {
      int id1 = entry.Id;
      string str = string.IsNullOrEmpty(entry.String) ? (string) null : entry.String;
      if (str == null)
      {
        this._strings.Remove(id1);
      }
      else
      {
        int id2;
        if (this.TryFindStringId(str, out id2))
        {
          if (id2 != id1)
          {
            this._strings.Remove(id2);
            this._strings.Add(id1, str);
          }
        }
        else
          this._strings.Add(id1, str);
      }
    }
    if (this._callback == null)
      return;
    if (this._network.IsClient && !this._initialized)
    {
      InitCallback callback = this._callback;
      if (callback != null)
        callback();
    }
    StringTableUpdateCallback updateCallback = this._updateCallback;
    if (updateCallback == null)
      return;
    updateCallback(message.Entries);
  }

  public void Reset()
  {
    this._strings.Clear();
    this._initialized = false;
    if (this.TryFindStringId("MsgStringTableEntries", out int _))
      return;
    this._strings.Add(0, "MsgStringTableEntries");
    if (!this._network.IsClient)
      return;
    StringTableUpdateCallback updateCallback = this._updateCallback;
    if (updateCallback == null)
      return;
    updateCallback(new MsgStringTableEntries.Entry[1]
    {
      new MsgStringTableEntries.Entry()
      {
        Id = 0,
        String = "MsgStringTableEntries"
      }
    });
  }

  public int AddString(string str)
  {
    if (this._network.IsClient)
      return -1;
    int id;
    if (this.TryFindStringId(str, out id))
      return id;
    do
    {
      ++this._lastStringIndex;
    }
    while (this._strings.ContainsKey(this._lastStringIndex));
    this._strings.Add(this._lastStringIndex, str);
    this.BroadcastTableUpdate(this._lastStringIndex, str);
    return this._lastStringIndex;
  }

  public void AddStringFixed(int id, string str)
  {
    if (this._network.IsClient)
      return;
    int id1;
    if (this.TryFindStringId(str, out id1))
    {
      if (id1 == id)
        return;
      this._strings.Remove(id1);
    }
    this._strings.Add(id, str);
    this.BroadcastTableUpdate(id, str);
  }

  public string? GetString(int id)
  {
    string str;
    return !this._strings.TryGetValue(id, out str) ? (string) null : str;
  }

  public bool TryGetString(int id, [NotNullWhen(true)] out string? str)
  {
    return this._strings.TryGetValue(id, out str);
  }

  public bool TryFindStringId(string str, out int id)
  {
    foreach (KeyValuePair<int, string> keyValuePair in this._strings)
    {
      if (!(keyValuePair.Value != str))
      {
        id = keyValuePair.Key;
        return true;
      }
    }
    id = 0;
    return false;
  }

  private void BroadcastTableUpdate(int id, string str)
  {
    if (this._network.IsClient || !this._network.IsRunning)
      return;
    MsgStringTableEntries message = new MsgStringTableEntries();
    message.Entries = new MsgStringTableEntries.Entry[1];
    message.Entries[0].Id = id;
    message.Entries[0].String = str;
    this._network.ServerSendToAll((NetMessage) message);
  }

  public void SendFullTable(INetChannel channel)
  {
    if (this._network.IsClient)
      return;
    MsgStringTableEntries message = new MsgStringTableEntries();
    int count = this._strings.Count;
    message.Entries = new MsgStringTableEntries.Entry[count];
    int index = 0;
    foreach (KeyValuePair<int, string> keyValuePair in this._strings)
    {
      message.Entries[index].Id = keyValuePair.Key;
      message.Entries[index].String = keyValuePair.Value;
      ++index;
    }
    this.Sawmill.Info($"Sending message name string table to {channel.RemoteEndPoint.Address}.");
    this._network.ServerSendMessage((NetMessage) message, channel);
  }
}
