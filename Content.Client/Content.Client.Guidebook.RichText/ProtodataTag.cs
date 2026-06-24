using System;
using System.Globalization;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook.RichText;

public sealed class ProtodataTag : IMarkupTagHandler
{
	[Dependency]
	private ILogManager _logMan;

	[Dependency]
	private IEntityManager _entMan;

	private ISawmill? _log;

	public string Name => "protodata";

	private ISawmill Log => _log ?? (_log = _logMan.GetSawmill("protodata_tag"));

	public string TextBefore(MarkupNode node)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		if (!((MarkupParameter)(ref node.Value)).TryGetString(ref text))
		{
			return string.Empty;
		}
		if (!node.Attributes.TryGetValue("comp", out var value))
		{
			return string.Empty;
		}
		if (!node.Attributes.TryGetValue("member", out var value2))
		{
			return string.Empty;
		}
		node.Attributes.TryGetValue("format", out var value3);
		if (!_entMan.System<GuidebookDataSystem>().TryGetValue(text, ((MarkupParameter)(ref value)).StringValue, ((MarkupParameter)(ref value2)).StringValue, out object value4))
		{
			Log.Error($"Failed to find protodata for {value}.{value2} in {text}");
			return "???";
		}
		if (string.IsNullOrEmpty(((MarkupParameter)(ref value3)).StringValue) || !(value4 is IFormattable formattable))
		{
			return value4?.ToString() ?? "NULL";
		}
		return formattable.ToString(((MarkupParameter)(ref value3)).StringValue, CultureInfo.CurrentCulture);
	}
}
