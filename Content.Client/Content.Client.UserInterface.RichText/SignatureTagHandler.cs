using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Client.Paper.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.RichText;

public sealed class SignatureTagHandler : IMarkupTagHandler
{
	private static int _signatureCounter = 0;

	public string Name => "signature";

	public static float FontLineHeight { get; set; } = 16f;

	private static int GetSignatureIndex(MarkupNode node)
	{
		return _signatureCounter++;
	}

	public static void ResetSignatureCounter()
	{
		_signatureCounter = 0;
	}

	private static int CountSignatureButtonsBefore(Control clickedButton)
	{
		int count = 0;
		Control val = clickedButton;
		while (val.Parent != null)
		{
			val = val.Parent;
		}
		bool found = false;
		CountSignatureButtonsRecursive(val, clickedButton, ref count, ref found);
		if (!found)
		{
			return 0;
		}
		return count;
	}

	private unsafe static void CountSignatureButtonsRecursive(Control control, Control target, ref int count, ref bool found)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (found)
		{
			return;
		}
		Button val = (Button)(object)((control is Button) ? control : null);
		if (val != null && val.Text == Loc.GetString("paper-signature-sign-button"))
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
				CountSignatureButtonsRecursive(((Enumerator)(ref enumerator)).Current, target, ref count, ref found);
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public SignatureTagHandler()
	{
		IoCManager.InjectDependencies<SignatureTagHandler>(this);
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
			Text = "Sign",
			MinSize = new Vector2(48f, FontLineHeight + 4f),
			MaxSize = new Vector2(48f, FontLineHeight + 4f),
			Margin = new Thickness(1f, 2f, 1f, 2f),
			StyleClasses = { "ButtonSquare" },
			TextAlign = (AlignMode)1
		};
		int signatureIndex = GetSignatureIndex(node);
		((Control)btn).Name = $"signature_{signatureIndex}";
		((BaseButton)btn).OnPressed += delegate
		{
			Control parent = ((Control)btn).Parent;
			while (parent != null && !(parent is PaperWindow))
			{
				parent = parent.Parent;
			}
			if (parent is PaperWindow paperWindow)
			{
				int signatureIndex2 = CountSignatureButtonsBefore((Control)(object)btn);
				paperWindow.SendSignatureRequest(signatureIndex2);
			}
		};
		control = (Control?)(object)btn;
		return true;
	}
}
