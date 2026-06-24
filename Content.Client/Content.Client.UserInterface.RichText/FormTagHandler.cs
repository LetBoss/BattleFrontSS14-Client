using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Paper.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

public sealed class FormTagHandler : IMarkupTagHandler
{
	private static int _formCounter = 0;

	private static readonly Dictionary<string, int> _formPositions = new Dictionary<string, int>();

	private static string _lastText = "";

	public string Name => "form";

	public static float FontLineHeight { get; set; } = 16f;

	public bool CanHandle(MarkupNode node)
	{
		if (!(node.Name == "form"))
		{
			return ((MarkupParameter)(ref node.Value)).StringValue?.StartsWith("__FORM_") ?? false;
		}
		return true;
	}

	private static int GetFormIndex(MarkupNode node)
	{
		return _formCounter++;
	}

	public static void ResetFormCounter()
	{
		_formCounter = 0;
	}

	private static int CountFormButtonsBefore(Control clickedButton)
	{
		int count = 0;
		Control val = clickedButton;
		while (val.Parent != null)
		{
			val = val.Parent;
		}
		bool found = false;
		CountFormButtonsRecursive(val, clickedButton, ref count, ref found);
		if (!found)
		{
			return 0;
		}
		return count;
	}

	private unsafe static void CountFormButtonsRecursive(Control control, Control target, ref int count, ref bool found)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (found)
		{
			return;
		}
		Button val = (Button)(object)((control is Button) ? control : null);
		if (val != null && val.Text == Loc.GetString("paper-form-fill-button"))
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
				CountFormButtonsRecursive(((Enumerator)(ref enumerator)).Current, target, ref count, ref found);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public static void SetFormText(string text)
	{
		if (_lastText != text)
		{
			_formPositions.Clear();
			_lastText = text;
			int startIndex = 0;
			int num = 0;
			while ((startIndex = text.IndexOf("[form]", startIndex)) != -1)
			{
				_formPositions[startIndex.ToString()] = num++;
				startIndex += 6;
			}
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		Button btn = new Button
		{
			Text = "Fill",
			MinSize = new Vector2(32f, FontLineHeight + 2f),
			MaxSize = new Vector2(32f, FontLineHeight + 2f),
			Margin = new Thickness(1f, 0f, 1f, 0f),
			StyleClasses = { "ButtonSquare" },
			TextAlign = (AlignMode)1
		};
		int formIndex = GetFormIndex(node);
		((Control)btn).Name = $"form_{formIndex}";
		((BaseButton)btn).OnPressed += delegate
		{
			Control parent = ((Control)btn).Parent;
			while (parent != null && !(parent is PaperWindow))
			{
				parent = parent.Parent;
			}
			if (parent is PaperWindow paperWindow)
			{
				int formIndex2 = CountFormButtonsBefore((Control)(object)btn);
				paperWindow.OpenFormDialog(formIndex2);
			}
		};
		control = (Control?)(object)btn;
		return true;
	}
}
