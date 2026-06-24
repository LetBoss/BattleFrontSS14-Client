using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Paper.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

public sealed class CheckTagHandler : IMarkupTagHandler
{
	private static int _checkCounter = 0;

	public string Name => "check";

	public static float FontLineHeight { get; set; } = 16f;

	private static int GetCheckIndex(MarkupNode node)
	{
		return _checkCounter++;
	}

	public static void ResetCheckCounter()
	{
		_checkCounter = 0;
	}

	private static int CountCheckButtonsBefore(Control clickedButton)
	{
		int count = 0;
		Control val = clickedButton;
		while (val.Parent != null)
		{
			val = val.Parent;
		}
		bool found = false;
		CountCheckButtonsRecursive(val, clickedButton, ref count, ref found);
		if (!found)
		{
			return 0;
		}
		return count;
	}

	private unsafe static void CountCheckButtonsRecursive(Control control, Control target, ref int count, ref bool found)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (found)
		{
			return;
		}
		Button val = (Button)(object)((control is Button) ? control : null);
		if (val != null && (val.Text == "☐" || val.Text == "✔" || val.Text == "✖"))
		{
			if (control == target)
			{
				found = true;
				return;
			}
			count++;
		}
		Enumerator enumerator = control.Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				CountCheckButtonsRecursive(((Enumerator)(ref enumerator)).Current, target, ref count, ref found);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void PushDrawContext(MarkupNode node, MarkupDrawingContext context)
	{
	}

	public void PopDrawContext(MarkupNode node, MarkupDrawingContext context)
	{
	}

	public string TextBefore(MarkupNode node)
	{
		return "";
	}

	public string TextAfter(MarkupNode node)
	{
		return "";
	}

	public bool TryCreateControl(MarkupNode node, [NotNullWhen(true)] out Control? control)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		Button btn = new Button
		{
			Text = "☐",
			MinSize = new Vector2(FontLineHeight + 2f, FontLineHeight + 2f),
			MaxSize = new Vector2(FontLineHeight + 2f, FontLineHeight + 2f),
			Margin = new Thickness(1f, 0f, 1f, 0f),
			StyleClasses = { "ButtonSquare" },
			TextAlign = (AlignMode)1
		};
		int checkIndex = GetCheckIndex(node);
		((Control)btn).Name = $"check_{checkIndex}";
		((BaseButton)btn).OnPressed += delegate
		{
			Control parent = ((Control)btn).Parent;
			while (parent != null && !(parent is PaperWindow))
			{
				parent = parent.Parent;
			}
			if (parent is PaperWindow paperWindow)
			{
				int checkIndex2 = CountCheckButtonsBefore((Control)(object)btn);
				paperWindow.OpenCheckDialog(checkIndex2);
			}
		};
		control = (Control?)(object)btn;
		return true;
	}

	private static string ReplaceNthCheckTag(string text, int index, string replacement)
	{
		int num = 0;
		int num2 = 0;
		while (num2 < text.Length)
		{
			int num3 = text.IndexOf("[check]", num2);
			if (num3 == -1)
			{
				break;
			}
			if (num == index)
			{
				return text.Substring(0, num3) + replacement + text.Substring(num3 + "[check]".Length);
			}
			num++;
			num2 = num3 + "[check]".Length;
		}
		return text;
	}
}
