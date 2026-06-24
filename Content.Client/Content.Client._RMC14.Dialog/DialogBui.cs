using System;
using Content.Shared._RMC14.Dialog;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Dialog;

public sealed class DialogBui : BoundUserInterface
{
	private RMCDialogWindow? _window;

	public DialogBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<RMCDialogWindow>((BoundUserInterface)(object)this);
		Refresh();
	}

	private unsafe void UpdateOptions(DialogComponent s)
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		RMCDialogWindow window = _window;
		if (window == null || !((BaseWindow)window).IsOpen)
		{
			return;
		}
		BoxContainer container = _window.Container;
		RMCDialogOptionsContainer container2 = container as RMCDialogOptionsContainer;
		if (container2 == null)
		{
			BoxContainer? container3 = _window.Container;
			if (container3 != null)
			{
				((Control)container3).Orphan();
			}
			_window.Container = null;
			container2 = new RMCDialogOptionsContainer();
			container2.Search.OnTextChanged += delegate(LineEditEventArgs args)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				Enumerator enumerator = ((Control)container2.Options).Children.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator)).MoveNext())
					{
						Control current = ((Enumerator)(ref enumerator)).Current;
						Button val2 = (Button)(object)((current is Button) ? current : null);
						if (val2 != null && val2.Text != null)
						{
							((Control)val2).Visible = val2.Text.Contains(args.Text, StringComparison.OrdinalIgnoreCase);
						}
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
				}
			};
			_window.Container = (BoxContainer?)(object)container2;
			((Control)_window).AddChild((Control)(object)_window.Container);
		}
		((DefaultWindow)_window).Title = s.Title;
		container2.Message.Text = s.Message.Text;
		RichTextLabel message = container2.Message;
		string text = container2.Message.Text;
		((Control)message).Visible = text != null && text.Length > 0;
		((Control)container2.Options).DisposeAllChildren();
		for (int num = 0; num < s.Options.Count; num++)
		{
			DialogOption dialogOption = s.Options[num];
			Button val = new Button
			{
				Text = dialogOption.Text,
				StyleClasses = { "OpenBoth" }
			};
			((Control)val.Label).AddStyleClass("CMAlignLeft");
			int index = num;
			((BaseButton)val).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DialogOptionBuiMsg(index));
			};
			((Control)container2.Options).AddChild((Control)(object)val);
		}
	}

	private void UpdateInput(DialogComponent s)
	{
		RMCDialogWindow window = _window;
		if (window == null || !((BaseWindow)window).IsOpen)
		{
			return;
		}
		BoxContainer container = _window.Container;
		RMCDialogInputContainer container2 = container as RMCDialogInputContainer;
		if (container2 == null)
		{
			BoxContainer? container3 = _window.Container;
			if (container3 != null)
			{
				((Control)container3).Orphan();
			}
			_window.Container = null;
			container2 = new RMCDialogInputContainer();
			container2.MessageLineEdit.OnTextEntered += delegate(LineEditEventArgs args)
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DialogInputBuiMsg(args.Text));
			};
			container2.MessageLineEdit.OnTextChanged += delegate(LineEditEventArgs args)
			{
				OnInputTextChanged(container2, args.Text.Length, s.CharacterLimit);
			};
			container2.MessageTextEdit.OnTextChanged += delegate(TextEditEventArgs args)
			{
				OnInputTextChanged(container2, (int)Rope.CalcTotalLength(args.TextRope), s.CharacterLimit);
			};
			((BaseButton)container2.CancelButton).OnPressed += delegate
			{
				((BoundUserInterface)this).Close();
			};
			((BaseButton)container2.OkButton).OnPressed += delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				DialogComponent componentOrNull = EntityManagerExt.GetComponentOrNull<DialogComponent>(base.EntMan, ((BoundUserInterface)this).Owner);
				string input = ((componentOrNull != null && componentOrNull.LargeInput) ? Rope.Collapse(container2.MessageTextEdit.TextRope) : container2.MessageLineEdit.Text);
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DialogInputBuiMsg(input));
			};
			_window.Container = (BoxContainer?)(object)container2;
			((Control)_window).AddChild((Control)(object)_window.Container);
			OnInputTextChanged(container2, 0, s.CharacterLimit);
		}
		((DefaultWindow)_window).Title = string.Empty;
		container2.MessageLabel.Text = s.Message.Text;
		((Control)container2.MessageLineEdit).Visible = !s.LargeInput;
		((Control)container2.MessageTextEdit).Visible = s.LargeInput;
		if (s.AutoFocus)
		{
			if (!s.LargeInput)
			{
				((Control)container2.MessageLineEdit).GrabKeyboardFocus();
			}
			else
			{
				((Control)container2.MessageTextEdit).GrabKeyboardFocus();
			}
		}
	}

	private void UpdateConfirm(DialogComponent s)
	{
		RMCDialogWindow window = _window;
		if (window == null || !((BaseWindow)window).IsOpen)
		{
			return;
		}
		RMCDialogConfirmContainer rMCDialogConfirmContainer = _window.Container as RMCDialogConfirmContainer;
		if (rMCDialogConfirmContainer == null)
		{
			BoxContainer? container = _window.Container;
			if (container != null)
			{
				((Control)container).Orphan();
			}
			_window.Container = null;
			rMCDialogConfirmContainer = new RMCDialogConfirmContainer();
			((BaseButton)rMCDialogConfirmContainer.CancelButton).OnPressed += delegate
			{
				((BoundUserInterface)this).Close();
			};
			((BaseButton)rMCDialogConfirmContainer.OkButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DialogConfirmBuiMsg());
			};
			_window.Container = (BoxContainer?)(object)rMCDialogConfirmContainer;
			((Control)_window).AddChild((Control)(object)_window.Container);
		}
		((DefaultWindow)_window).Title = s.Title;
		rMCDialogConfirmContainer.MessageLabel.Text = s.Message.Text;
	}

	public void Refresh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DialogComponent dialogComponent = default(DialogComponent);
		if (base.EntMan.TryGetComponent<DialogComponent>(((BoundUserInterface)this).Owner, ref dialogComponent))
		{
			switch (dialogComponent.DialogType)
			{
			case DialogType.Options:
				UpdateOptions(dialogComponent);
				break;
			case DialogType.Input:
				UpdateInput(dialogComponent);
				break;
			case DialogType.Confirm:
				UpdateConfirm(dialogComponent);
				break;
			}
			RMCDialogWindow? window = _window;
			if (window != null)
			{
				((BaseWindow)window).OpenCentered();
			}
		}
	}

	private void OnInputTextChanged(RMCDialogInputContainer container, int textLength, int max)
	{
		container.CharacterCount.Text = $"{textLength} / {max}";
		((BaseButton)container.OkButton).Disabled = textLength > max;
	}
}
