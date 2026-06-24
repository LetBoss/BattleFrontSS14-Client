using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Sprite;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Shared._RMC14.Light;

public sealed class RMCLightOffsetSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCSpriteSystem _sprite;

	private readonly HashSet<EntityUid> ToUpdate = new HashSet<EntityUid>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCLightOffsetComponent, MapInitEvent>((EntityEventRefHandler<RMCLightOffsetComponent, MapInitEvent>)OnLightUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCLightOffsetComponent, EntParentChangedMessage>((EntityEventRefHandler<RMCLightOffsetComponent, EntParentChangedMessage>)OnLightUpdate, (Type[])null, (Type[])null);
	}

	private void OnLightUpdate<T>(Entity<RMCLightOffsetComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected I4, but got Unknown
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		MetaDataComponent metaData = default(MetaDataComponent);
		if (!((EntitySystem)this).TryComp(Entity<RMCLightOffsetComponent>.op_Implicit(ent), ref metaData) || (int)metaData.EntityLifeStage < 3)
		{
			return;
		}
		ToUpdate.Add(Entity<RMCLightOffsetComponent>.op_Implicit(ent));
		if (!_net.IsClient && !((EntitySystem)this).TerminatingOrDeleted(Entity<RMCLightOffsetComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			SpriteSetRenderOrderComponent sprite = ((EntitySystem)this).EnsureComp<SpriteSetRenderOrderComponent>(Entity<RMCLightOffsetComponent>.op_Implicit(ent));
			Angle localRotation = ((EntitySystem)this).Transform(Entity<RMCLightOffsetComponent>.op_Implicit(ent)).LocalRotation;
			Direction dir = ((Angle)(ref localRotation)).GetDir();
			switch ((int)dir)
			{
			case 0:
				_sprite.SetOffset(Entity<RMCLightOffsetComponent>.op_Implicit(ent), new Vector2(0.45f, -0.32f));
				break;
			case 2:
				_sprite.SetOffset(Entity<RMCLightOffsetComponent>.op_Implicit(ent), new Vector2(0.7f, -1.45f));
				break;
			case 4:
				_sprite.SetOffset(Entity<RMCLightOffsetComponent>.op_Implicit(ent), new Vector2(-0.5f, -1.5f));
				break;
			case 6:
				_sprite.SetOffset(Entity<RMCLightOffsetComponent>.op_Implicit(ent), new Vector2(-0.7f, -0.4f));
				break;
			}
			((EntitySystem)this).Dirty(Entity<RMCLightOffsetComponent>.op_Implicit(ent), (IComponent)(object)sprite, (MetaDataComponent)null);
		}
	}
}
