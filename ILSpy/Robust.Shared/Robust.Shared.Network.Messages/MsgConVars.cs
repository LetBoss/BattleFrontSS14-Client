using System;
using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Network.Messages;

public sealed class MsgConVars : NetMessage
{
	private enum CvarType : byte
	{
		None,
		Int,
		Long,
		Bool,
		String,
		Float,
		Double
	}

	private const int MaxMessageSize = 32768;

	private const int MaxNameSize = 128;

	private const int MaxStringValSize = 1024;

	public GameTick Tick;

	public List<(string name, object value)> NetworkedVars;

	public override MsgGroups MsgGroup => MsgGroups.String;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		if (((NetBuffer)buffer).LengthBytes > 32768)
		{
			Logger.WarningS("net", $"{base.MsgChannel}: received a large {"MsgConVars"}, {((NetBuffer)buffer).LengthBytes}B > {32768}B");
		}
		Tick = new GameTick(((NetBuffer)buffer).ReadVariableUInt32());
		short num = ((NetBuffer)buffer).ReadInt16();
		NetworkedVars = new List<(string, object)>(num);
		for (int i = 0; i < num; i++)
		{
			int num2 = ((NetBuffer)buffer).PeekStringSize();
			if (0 >= num2 || num2 > 128)
			{
				throw new InvalidOperationException($"Cvar name size '{num2}' is out of bounds (1-{128} bytes).");
			}
			string text = ((NetBuffer)buffer).ReadString();
			CvarType cvarType = (CvarType)((NetBuffer)buffer).ReadByte();
			object item;
			switch (cvarType)
			{
			case CvarType.Int:
				item = ((NetBuffer)buffer).ReadInt32();
				break;
			case CvarType.Long:
				item = ((NetBuffer)buffer).ReadInt64();
				break;
			case CvarType.Bool:
				item = ((NetBuffer)buffer).ReadBoolean();
				break;
			case CvarType.String:
			{
				int num3 = ((NetBuffer)buffer).PeekStringSize();
				if (0 > num3 || num3 > 1024)
				{
					throw new InvalidOperationException($"Cvar string value size '{num2}' for cvar '{text}' is out of bounds (0-{1024} bytes).");
				}
				item = ((NetBuffer)buffer).ReadString();
				break;
			}
			case CvarType.Float:
				item = ((NetBuffer)buffer).ReadFloat();
				break;
			case CvarType.Double:
				item = ((NetBuffer)buffer).ReadDouble();
				break;
			default:
				throw new ArgumentOutOfRangeException("value", cvarType, "CVar " + text + " is not of a valid CVar type!");
			}
			NetworkedVars.Add((text, item));
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		if (NetworkedVars == null)
		{
			throw new InvalidOperationException("NetworkedVars collection is null.");
		}
		if (NetworkedVars.Count > 32767)
		{
			throw new InvalidOperationException($"{"NetworkedVars"} collection count is greater than {short.MaxValue}.");
		}
		((NetBuffer)buffer).WriteVariableUInt32(Tick.Value);
		((NetBuffer)buffer).Write((short)NetworkedVars.Count);
		foreach (var (text, obj) in NetworkedVars)
		{
			((NetBuffer)buffer).Write(text);
			if (!(obj is int num))
			{
				if (!(obj is long num2))
				{
					if (!(obj is bool flag))
					{
						if (!(obj is string text2))
						{
							if (!(obj is float num3))
							{
								if (!(obj is double num4))
								{
									throw new ArgumentOutOfRangeException("value", obj.GetType(), "CVar " + text + " is not of a valid CVar type!");
								}
								((NetBuffer)buffer).Write((byte)6);
								((NetBuffer)buffer).Write(num4);
							}
							else
							{
								((NetBuffer)buffer).Write((byte)5);
								((NetBuffer)buffer).Write(num3);
							}
						}
						else
						{
							((NetBuffer)buffer).Write((byte)4);
							((NetBuffer)buffer).Write(text2);
						}
					}
					else
					{
						((NetBuffer)buffer).Write((byte)3);
						((NetBuffer)buffer).Write(flag);
					}
				}
				else
				{
					((NetBuffer)buffer).Write((byte)2);
					((NetBuffer)buffer).Write(num2);
				}
			}
			else
			{
				((NetBuffer)buffer).Write((byte)1);
				((NetBuffer)buffer).Write(num);
			}
		}
	}
}
