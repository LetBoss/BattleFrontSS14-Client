using Content.Client.Message;
using Content.Shared.GPS.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Client.GPS.UI;

public sealed class HandheldGpsStatusControl : Control
{
	private readonly Entity<HandheldGPSComponent> _parent;

	private readonly RichTextLabel _label;

	private float _updateDif;

	private readonly IEntityManager _entMan;

	private readonly SharedTransformSystem _transform;

	public HandheldGpsStatusControl(Entity<HandheldGPSComponent> parent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		_parent = parent;
		_entMan = IoCManager.Resolve<IEntityManager>();
		_transform = (SharedTransformSystem)(object)_entMan.System<TransformSystem>();
		_label = new RichTextLabel
		{
			StyleClasses = { "ItemStatus" }
		};
		((Control)this).AddChild((Control)(object)_label);
		UpdateGpsDetails();
	}

	protected override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		((Control)this).FrameUpdate(args);
		if ((int)((Component)_parent.Comp).LifeStage > 6)
		{
			((Control)_label).Visible = false;
			return;
		}
		_updateDif += ((FrameEventArgs)(ref args)).DeltaSeconds;
		if (!(_updateDif < _parent.Comp.UpdateRate))
		{
			_updateDif -= _parent.Comp.UpdateRate;
			UpdateGpsDetails();
		}
	}

	private void UpdateGpsDetails()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		string item = "Error";
		TransformComponent val = default(TransformComponent);
		if (_entMan.TryGetComponent<TransformComponent>(Entity<HandheldGPSComponent>.op_Implicit(_parent), ref val))
		{
			MapCoordinates mapCoordinates = _transform.GetMapCoordinates(_parent.Owner, val);
			int value = (int)((MapCoordinates)(ref mapCoordinates)).X;
			int value2 = (int)((MapCoordinates)(ref mapCoordinates)).Y;
			item = $"({value}, {value2})";
		}
		_label.SetMarkup(Loc.GetString("handheld-gps-coordinates-title", new(string, object)[1] { ("coordinates", item) }));
	}
}
