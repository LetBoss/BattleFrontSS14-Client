using System;
using Content.Shared.Beam;
using Content.Shared.Beam.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Beam;

public sealed class BeamSystem : SharedBeamSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<BeamVisualizerEvent>((EntityEventHandler<BeamVisualizerEvent>)BeamVisualizerMessage, (Type[])null, (Type[])null);
	}

	private void BeamVisualizerMessage(BeamVisualizerEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((EntitySystem)this).GetEntity(args.Beam);
		SpriteComponent val = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(entity, ref val))
		{
			_sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((entity, val)), args.UserAngle);
			if (args.BodyState != null)
			{
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity, val)), 0, StateId.op_Implicit(args.BodyState));
				val.LayerSetShader(0, args.Shader);
			}
		}
	}
}
