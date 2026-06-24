using Lidgren.Network;
using Robust.Shared.Console;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgConCompletionResp : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public int Seq { get; set; }

	public CompletionResult Result { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Seq = ((NetBuffer)buffer).ReadInt32();
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		CompletionOption[] array = new CompletionOption[num];
		for (int i = 0; i < num; i++)
		{
			string value = ((NetBuffer)buffer).ReadString();
			string text = ((NetBuffer)buffer).ReadString();
			int flags = ((NetBuffer)buffer).ReadInt32();
			array[i] = new CompletionOption(value, (text == "") ? null : text, (CompletionOptionFlags)flags);
		}
		string text2 = ((NetBuffer)buffer).ReadString();
		Result = new CompletionResult(array, (text2 == "") ? null : text2);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Seq);
		((NetBuffer)buffer).WriteVariableInt32(Result.Options.Length);
		CompletionOption[] options = Result.Options;
		for (int i = 0; i < options.Length; i++)
		{
			CompletionOption completionOption = options[i];
			((NetBuffer)buffer).Write(completionOption.Value);
			((NetBuffer)buffer).Write(completionOption.Hint);
			((NetBuffer)buffer).Write((int)completionOption.Flags);
		}
		((NetBuffer)buffer).Write(Result.Hint);
	}
}
