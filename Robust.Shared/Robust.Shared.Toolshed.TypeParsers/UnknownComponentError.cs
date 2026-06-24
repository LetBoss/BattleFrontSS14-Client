using System.Diagnostics;
using System.Globalization;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

public record struct UnknownComponentError(string Component) : IConError
{
	public string? Expression { get; set; } = null;

	public Vector2i? IssueSpan { get; set; } = null;

	public StackTrace? Trace { get; set; } = null;

	public FormattedMessage DescribeInner()
	{
		FormattedMessage formattedMessage = FormattedMessage.FromUnformatted("Unknown component " + Component + ". For a list of all components, try types:components.");
		if (Component.EndsWith("component", ignoreCase: true, CultureInfo.InvariantCulture))
		{
			formattedMessage.PushNewline();
			string component = Component;
			int length = "component".Length;
			formattedMessage.AddText("Do not specify the word `Component` in the argument. Maybe try " + component.Substring(0, component.Length - length) + "?");
		}
		return formattedMessage;
	}
}
