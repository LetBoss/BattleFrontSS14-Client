using System;
using System.Text;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

public sealed class ScrambleTag : IMarkupTag, IMarkupTagHandler
{
	[Dependency]
	private IGameTiming _timing;

	private const int MaxScrambleLength = 32;

	public string Name => "scramble";

	public string TextBefore(MarkupNode node)
	{
		long? num = default(long?);
		long? num2 = default(long?);
		string text = default(string);
		if (!node.Attributes.TryGetValue("rate", out var value) || !((MarkupParameter)(ref value)).TryGetLong(ref num) || !node.Attributes.TryGetValue("length", out var value2) || !((MarkupParameter)(ref value2)).TryGetLong(ref num2) || !node.Attributes.TryGetValue("chars", out var value3) || !((MarkupParameter)(ref value3)).TryGetString(ref text))
		{
			return string.Empty;
		}
		Random random = new Random((int)(_timing.CurTime.TotalMilliseconds / (double?)num).Value + ((object)node).GetHashCode());
		char[] array = text.ToCharArray();
		float num3 = MathF.Min(num2.Value, 32f);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; (float)i < num3; i++)
		{
			int num4 = random.Next() % array.Length;
			stringBuilder.Append(array[num4]);
		}
		return stringBuilder.ToString();
	}
}
