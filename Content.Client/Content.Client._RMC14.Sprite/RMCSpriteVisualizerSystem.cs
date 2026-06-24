using System;
using System.Numerics;
using Content.Client.Outline;
using Content.Shared._RMC14.Sprite;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Sprite;

public sealed class RMCSpriteVisualizerSystem : VisualizerSystem<SpriteSetRenderOrderComponent>
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).UpdatesAfter.Add(typeof(InteractionOutlineSystem));
	}

	protected override void OnAppearanceChange(EntityUid uid, SpriteSetRenderOrderComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			int renderOrder = default(int);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)SpriteSetRenderOrderComponent.Appearance.Key, ref renderOrder, args.Component))
			{
				args.Sprite.RenderOrder = (uint)renderOrder;
			}
			Vector2 vector = default(Vector2);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<Vector2>(uid, (Enum)SpriteSetRenderOrderComponent.Appearance.Offset, ref vector, args.Component))
			{
				_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), vector);
			}
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<SpriteSetRenderOrderComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<SpriteSetRenderOrderComponent, SpriteComponent>();
		EntityUid item = default(EntityUid);
		SpriteSetRenderOrderComponent spriteSetRenderOrderComponent = default(SpriteSetRenderOrderComponent);
		SpriteComponent val2 = default(SpriteComponent);
		while (val.MoveNext(ref item, ref spriteSetRenderOrderComponent, ref val2))
		{
			if (spriteSetRenderOrderComponent.RenderOrder.HasValue)
			{
				val2.RenderOrder = (uint)spriteSetRenderOrderComponent.RenderOrder.Value;
			}
			if (spriteSetRenderOrderComponent.Offset.HasValue)
			{
				_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((item, val2)), spriteSetRenderOrderComponent.Offset.Value);
			}
		}
	}
}
