using System;
using Content.Shared.Delivery;
using Content.Shared.StatusIcon;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Delivery;

public sealed class DeliveryVisualizerSystem : VisualizerSystem<DeliveryComponent>
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IPrototypeManager _prototype;

	private static readonly ProtoId<JobIconPrototype> UnknownIcon = ProtoId<JobIconPrototype>.op_Implicit("JobIconUnknown");

	protected override void OnAppearanceChange(EntityUid uid, DeliveryComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			string text = default(string);
			_appearance.TryGetData<string>(uid, (Enum)DeliveryVisuals.JobIcon, ref text, args.Component);
			if (string.IsNullOrEmpty(text))
			{
				text = ProtoId<JobIconPrototype>.op_Implicit(UnknownIcon);
			}
			JobIconPrototype jobIconPrototype = default(JobIconPrototype);
			if (!_prototype.TryIndex<JobIconPrototype>(text, ref jobIconPrototype))
			{
				base.SpriteSystem.LayerSetTexture(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DeliveryVisualLayers.JobStamp, base.SpriteSystem.Frame0(_prototype.Index<JobIconPrototype>(UnknownIcon).Icon));
			}
			else
			{
				base.SpriteSystem.LayerSetTexture(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)DeliveryVisualLayers.JobStamp, base.SpriteSystem.Frame0(jobIconPrototype.Icon));
			}
		}
	}
}
