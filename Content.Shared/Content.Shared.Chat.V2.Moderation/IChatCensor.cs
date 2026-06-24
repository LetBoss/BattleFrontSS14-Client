namespace Content.Shared.Chat.V2.Moderation;

public interface IChatCensor
{
	bool Censor(string input, out string output, char replaceWith = '*');
}
