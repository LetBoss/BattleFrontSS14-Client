using Robust.Client.Input;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook.Richtext;

public sealed class KeyBindTag : IMarkupTagHandler
{
	[Dependency]
	private IInputManager _inputManager;

	public string Name => "keybind";

	public string TextBefore(MarkupNode node)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		if (!((MarkupParameter)(ref node.Value)).TryGetString(ref text))
		{
			return "";
		}
		IKeyBinding val = default(IKeyBinding);
		if (!_inputManager.TryGetKeyBinding(BoundKeyFunction.op_Implicit(text), ref val))
		{
			return text;
		}
		return val.GetKeyString();
	}
}
