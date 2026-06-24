using System.Collections.Generic;

namespace Content.Shared.Chat.V2.Moderation;

public sealed class CompoundChatCensor(IEnumerable<IChatCensor> censors) : IChatCensor
{
	public bool Censor(string input, out string output, char replaceWith = '*')
	{
		bool censored = false;
		foreach (IChatCensor item in censors)
		{
			if (item.Censor(input, out output, replaceWith))
			{
				censored = true;
			}
		}
		output = input;
		return censored;
	}
}
