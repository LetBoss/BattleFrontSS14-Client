using System.Collections.Generic;

namespace Content.Shared.Chat.V2.Moderation;

public sealed class ChatCensorFactory
{
	private List<IChatCensor> _censors = new List<IChatCensor>();

	public void With(IChatCensor censor)
	{
		_censors.Add(censor);
	}

	public IChatCensor Build()
	{
		return new CompoundChatCensor(_censors.ToArray());
	}

	public bool Reset()
	{
		bool result = _censors.Count > 0;
		_censors = new List<IChatCensor>();
		return result;
	}
}
