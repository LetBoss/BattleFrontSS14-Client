using System.Collections.Generic;
using System.Collections.Immutable;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptCompletionResponse : NetMessage
{
	public sealed class LiteResult
	{
		public string DisplayText;

		public string DisplayTextPrefix;

		public string DisplayTextSuffix;

		public string InlineDescription;

		public ImmutableArray<string> Tags;

		public ImmutableDictionary<string, string> Properties;

		public LiteResult(string displayText, string displayTextPrefix, string displayTextSuffix, string inlineDescription, ImmutableArray<string> tags, ImmutableDictionary<string, string> properties)
		{
			DisplayText = displayText;
			DisplayTextPrefix = displayTextPrefix;
			DisplayTextSuffix = displayTextSuffix;
			InlineDescription = inlineDescription;
			Tags = tags;
			Properties = properties;
		}

		public LiteResult(NetIncomingMessage buffer)
		{
			DisplayText = ((NetBuffer)buffer).ReadString();
			DisplayTextPrefix = ((NetBuffer)buffer).ReadString();
			DisplayTextSuffix = ((NetBuffer)buffer).ReadString();
			InlineDescription = ((NetBuffer)buffer).ReadString();
			int num = ((NetBuffer)buffer).ReadInt32();
			ImmutableArray<string>.Builder builder = ImmutableArray.CreateBuilder<string>();
			for (int i = 0; i < num; i++)
			{
				builder.Add(((NetBuffer)buffer).ReadString());
			}
			Tags = builder.ToImmutable();
			num = ((NetBuffer)buffer).ReadInt32();
			ImmutableDictionary<string, string>.Builder builder2 = ImmutableDictionary.CreateBuilder<string, string>();
			for (int j = 0; j < num; j++)
			{
				builder2.Add(((NetBuffer)buffer).ReadString(), ((NetBuffer)buffer).ReadString());
			}
			Properties = builder2.ToImmutable();
		}

		public void WriteToBuffer(NetOutgoingMessage buffer)
		{
			((NetBuffer)buffer).Write(DisplayText);
			((NetBuffer)buffer).Write(DisplayTextPrefix);
			((NetBuffer)buffer).Write(DisplayTextSuffix);
			((NetBuffer)buffer).Write(InlineDescription);
			((NetBuffer)buffer).Write(Tags.Length);
			ImmutableArray<string>.Enumerator enumerator = Tags.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				((NetBuffer)buffer).Write(current);
			}
			((NetBuffer)buffer).Write(Properties.Count);
			foreach (KeyValuePair<string, string> property in Properties)
			{
				((NetBuffer)buffer).Write(property.Key);
				((NetBuffer)buffer).Write(property.Value);
			}
		}
	}

	public ImmutableArray<LiteResult> Results;

	public override MsgGroups MsgGroup => MsgGroups.Command;

	public int ScriptSession { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		ScriptSession = ((NetBuffer)buffer).ReadInt32();
		int num = ((NetBuffer)buffer).ReadInt32();
		ImmutableArray<LiteResult>.Builder builder = ImmutableArray.CreateBuilder<LiteResult>();
		for (int i = 0; i < num; i++)
		{
			LiteResult item = new LiteResult(buffer);
			builder.Add(item);
		}
		Results = builder.ToImmutable();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(ScriptSession);
		((NetBuffer)buffer).Write(Results.Length);
		ImmutableArray<LiteResult>.Enumerator enumerator = Results.GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.WriteToBuffer(buffer);
		}
	}
}
