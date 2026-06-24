using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Log;

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

	internal Dictionary<int, string> Strings => _strings;

	public static int InvalidStringId => -1;

	public StringTable(INetManager network)
	{
		_network = network;
		_strings = new Dictionary<int, string>();
	}

	public void Initialize(InitCallback? callback = null, StringTableUpdateCallback? updateCallback = null)
	{
		_callback = callback;
		_updateCallback = updateCallback;
		_network.RegisterNetMessage<MsgStringTableEntries>(ReceiveEntries, NetMessageAccept.Client | NetMessageAccept.Handshake);
		Reset();
		_initialized = true;
	}

	private void ReceiveEntries(MsgStringTableEntries message)
	{
		Sawmill.Info("Received message name string table.");
		MsgStringTableEntries.Entry[] entries = message.Entries;
		for (int i = 0; i < entries.Length; i++)
		{
			MsgStringTableEntries.Entry entry = entries[i];
			int id = entry.Id;
			string text = (string.IsNullOrEmpty(entry.String) ? null : entry.String);
			int id2;
			if (text == null)
			{
				_strings.Remove(id);
			}
			else if (TryFindStringId(text, out id2))
			{
				if (id2 != id)
				{
					_strings.Remove(id2);
					_strings.Add(id, text);
				}
			}
			else
			{
				_strings.Add(id, text);
			}
		}
		if (_callback != null)
		{
			if (_network.IsClient && !_initialized)
			{
				_callback?.Invoke();
			}
			_updateCallback?.Invoke(message.Entries);
		}
	}

	public void Reset()
	{
		_strings.Clear();
		_initialized = false;
		if (!TryFindStringId("MsgStringTableEntries", out var _))
		{
			_strings.Add(0, "MsgStringTableEntries");
			if (_network.IsClient)
			{
				_updateCallback?.Invoke(new MsgStringTableEntries.Entry[1]
				{
					new MsgStringTableEntries.Entry
					{
						Id = 0,
						String = "MsgStringTableEntries"
					}
				});
			}
		}
	}

	public int AddString(string str)
	{
		if (_network.IsClient)
		{
			return -1;
		}
		if (TryFindStringId(str, out var id))
		{
			return id;
		}
		do
		{
			_lastStringIndex++;
		}
		while (_strings.ContainsKey(_lastStringIndex));
		_strings.Add(_lastStringIndex, str);
		BroadcastTableUpdate(_lastStringIndex, str);
		return _lastStringIndex;
	}

	public void AddStringFixed(int id, string str)
	{
		if (_network.IsClient)
		{
			return;
		}
		if (TryFindStringId(str, out var id2))
		{
			if (id2 == id)
			{
				return;
			}
			_strings.Remove(id2);
		}
		_strings.Add(id, str);
		BroadcastTableUpdate(id, str);
	}

	public string? GetString(int id)
	{
		if (!_strings.TryGetValue(id, out string value))
		{
			return null;
		}
		return value;
	}

	public bool TryGetString(int id, [NotNullWhen(true)] out string? str)
	{
		return _strings.TryGetValue(id, out str);
	}

	public bool TryFindStringId(string str, out int id)
	{
		foreach (KeyValuePair<int, string> @string in _strings)
		{
			if (!(@string.Value != str))
			{
				id = @string.Key;
				return true;
			}
		}
		id = 0;
		return false;
	}

	private void BroadcastTableUpdate(int id, string str)
	{
		if (!_network.IsClient && _network.IsRunning)
		{
			MsgStringTableEntries msgStringTableEntries = new MsgStringTableEntries();
			msgStringTableEntries.Entries = new MsgStringTableEntries.Entry[1];
			msgStringTableEntries.Entries[0].Id = id;
			msgStringTableEntries.Entries[0].String = str;
			_network.ServerSendToAll(msgStringTableEntries);
		}
	}

	public void SendFullTable(INetChannel channel)
	{
		if (_network.IsClient)
		{
			return;
		}
		MsgStringTableEntries msgStringTableEntries = new MsgStringTableEntries();
		int count = _strings.Count;
		msgStringTableEntries.Entries = new MsgStringTableEntries.Entry[count];
		int num = 0;
		foreach (KeyValuePair<int, string> @string in _strings)
		{
			msgStringTableEntries.Entries[num].Id = @string.Key;
			msgStringTableEntries.Entries[num].String = @string.Value;
			num++;
		}
		Sawmill.Info($"Sending message name string table to {channel.RemoteEndPoint.Address}.");
		_network.ServerSendMessage(msgStringTableEntries, channel);
	}
}
