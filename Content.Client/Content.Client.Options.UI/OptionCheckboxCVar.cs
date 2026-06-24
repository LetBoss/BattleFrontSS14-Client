using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;

namespace Content.Client.Options.UI;

public sealed class OptionCheckboxCVar : BaseOptionCVar<bool>
{
	private readonly CheckBox _checkBox;

	private readonly bool _invert;

	protected override bool Value
	{
		get
		{
			return ((BaseButton)_checkBox).Pressed ^ _invert;
		}
		set
		{
			((BaseButton)_checkBox).Pressed = value ^ _invert;
		}
	}

	public OptionCheckboxCVar(OptionsTabControlRow controller, IConfigurationManager cfg, CVarDef<bool> cVar, CheckBox checkBox, bool invert)
		: base(controller, cfg, cVar)
	{
		_checkBox = checkBox;
		_invert = invert;
		((BaseButton)checkBox).OnToggled += delegate
		{
			ValueChanged();
		};
	}
}
