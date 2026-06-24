using System;
using Content.Client.Items;
using Content.Client.Message;
using Content.Shared.Crayon;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Crayon;

public sealed class CrayonSystem : SharedCrayonSystem
{
	private sealed class StatusControl : Control
	{
		private readonly CrayonComponent _parent;

		private readonly RichTextLabel _label;

		public StatusControl(CrayonComponent parent)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			_parent = parent;
			_label = new RichTextLabel
			{
				StyleClasses = { "ItemStatus" }
			};
			((Control)this).AddChild((Control)(object)_label);
			parent.UIUpdateNeeded = true;
		}

		protected override void FrameUpdate(FrameEventArgs args)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			((Control)this).FrameUpdate(args);
			if (_parent.UIUpdateNeeded)
			{
				_parent.UIUpdateNeeded = false;
				_label.SetMarkup(Loc.GetString("crayon-drawing-label", new(string, object)[4]
				{
					("color", _parent.Color),
					("state", _parent.SelectedState),
					("charges", _parent.Charges),
					("capacity", _parent.Capacity)
				}));
			}
		}
	}

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CrayonComponent, ComponentHandleState>((ComponentEventRefHandler<CrayonComponent, ComponentHandleState>)OnCrayonHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).Subs.ItemStatus<CrayonComponent>((Func<Entity<CrayonComponent>, Control?>)((Entity<CrayonComponent> ent) => (Control?)(object)new StatusControl(Entity<CrayonComponent>.op_Implicit(ent))));
	}

	private static void OnCrayonHandleState(EntityUid uid, CrayonComponent component, ref ComponentHandleState args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is CrayonComponentState crayonComponentState)
		{
			component.Color = crayonComponentState.Color;
			component.SelectedState = crayonComponentState.State;
			component.Charges = crayonComponentState.Charges;
			component.Capacity = crayonComponentState.Capacity;
			component.UIUpdateNeeded = true;
		}
	}
}
