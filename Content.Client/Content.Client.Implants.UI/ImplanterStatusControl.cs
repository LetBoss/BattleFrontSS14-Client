using Content.Client.Message;
using Content.Client.UserInterface.Controls;
using Content.Shared.Implants.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Implants.UI;

public sealed class ImplanterStatusControl : Control
{
	[Dependency]
	private IPrototypeManager _prototype;

	private readonly ImplanterComponent _parent;

	private readonly RichTextLabel _label;

	public ImplanterStatusControl(ImplanterComponent parent)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		IoCManager.InjectDependencies<ImplanterStatusControl>(this);
		_parent = parent;
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)_label).MaxWidth = 350f;
		ClipControl clipControl = new ClipControl();
		((Control)clipControl).Children.Add((Control)(object)_label);
		((Control)this).AddChild((Control)(object)clipControl);
		Update();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FrameUpdate(args);
		if (_parent.UiUpdateNeeded)
		{
			Update();
		}
	}

	private void Update()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		_parent.UiUpdateNeeded = false;
		string item = _parent.CurrentMode switch
		{
			ImplanterToggleMode.Draw => Loc.GetString("implanter-draw-text"), 
			ImplanterToggleMode.Inject => Loc.GetString("implanter-inject-text"), 
			_ => Loc.GetString("injector-invalid-injector-toggle-mode"), 
		};
		if (_parent.CurrentMode == ImplanterToggleMode.Draw)
		{
			EntityPrototype val = default(EntityPrototype);
			string item2 = ((!_parent.DeimplantChosen.HasValue) ? Loc.GetString("implanter-empty-text") : (_prototype.TryIndex(_parent.DeimplantChosen.Value, ref val) ? val.Name : Loc.GetString("implanter-empty-text")));
			_label.SetMarkup(Loc.GetString("implanter-label-draw", new(string, object)[2]
			{
				("implantName", item2),
				("modeString", item)
			}));
		}
		else
		{
			string item3 = (_parent.ImplanterSlot.HasItem ? _parent.ImplantData.Item1 : Loc.GetString("implanter-empty-text"));
			_label.SetMarkup(Loc.GetString("implanter-label-inject", new(string, object)[2]
			{
				("implantName", item3),
				("modeString", item)
			}));
		}
	}
}
