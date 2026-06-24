using System;
using Content.Shared.Fax;
using Content.Shared.Fax.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Fax.System;

public sealed class FaxVisualsSystem : EntitySystem
{
	[Dependency]
	private AnimationPlayerSystem _player;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FaxMachineComponent, AppearanceChangeEvent>((ComponentEventRefHandler<FaxMachineComponent, AppearanceChangeEvent>)OnAppearanceChanged, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChanged(EntityUid uid, FaxMachineComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00c3: Expected O, but got Unknown
		FaxMachineVisualState faxMachineVisualState = default(FaxMachineVisualState);
		if (args.Sprite != null && !_player.HasRunningAnimation(uid, "faxecute") && _appearance.TryGetData<FaxMachineVisualState>(uid, (Enum)FaxMachineVisuals.VisualState, ref faxMachineVisualState, (AppearanceComponent)null) && faxMachineVisualState == FaxMachineVisualState.Inserting)
		{
			_player.Play(uid, new Animation
			{
				Length = TimeSpan.FromSeconds(2.4),
				AnimationTracks = { (AnimationTrack)new AnimationTrackSpriteFlick
				{
					LayerKey = FaxMachineVisuals.VisualState,
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit(component.InsertingState), 0f)
					},
					KeyFrames = 
					{
						new KeyFrame(StateId.op_Implicit("icon"), 2.4f)
					}
				} }
			}, "faxecute");
		}
	}
}
