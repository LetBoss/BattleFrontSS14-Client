using System.Diagnostics.CodeAnalysis;
using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Input;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook.RichText;

public sealed class TextLinkTag : IMarkupTagHandler
{
	private static readonly ISawmill Sawmill = Logger.GetSawmill("guidebook.textlink");

	public static Color LinkColor => Color.CornflowerBlue;

	public string Name => "textlink";

	public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		string link = default(string);
		if (!((MarkupParameter)(ref node.Value)).TryGetString(ref text) || !node.Attributes.TryGetValue("link", out var value) || !((MarkupParameter)(ref value)).TryGetString(ref link))
		{
			control = null;
			return false;
		}
		Label label = new Label();
		label.Text = text;
		((Control)label).MouseFilter = (MouseFilterMode)0;
		label.FontColorOverride = LinkColor;
		((Control)label).DefaultCursorShape = (CursorShape)3;
		((Control)label).OnMouseEntered += delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			label.FontColorOverride = Color.LightSkyBlue;
		};
		((Control)label).OnMouseExited += delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			label.FontColorOverride = Color.CornflowerBlue;
		};
		((Control)label).OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
		{
			OnKeybindDown(args, link, (Control?)(object)label);
		};
		control = (Control?)(object)label;
		return true;
	}

	private void OnKeybindDown(GUIBoundKeyEventArgs args, string link, Control? control)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!(((BoundKeyEventArgs)args).Function != EngineKeyFunctions.UIClick) && control != null)
		{
			if (control.TryGetParentHandler<ILinkClickHandler>(out ILinkClickHandler result))
			{
				result.HandleClick(link);
			}
			else
			{
				Sawmill.Warning("Warning! No valid ILinkClickHandler found.");
			}
		}
	}
}
