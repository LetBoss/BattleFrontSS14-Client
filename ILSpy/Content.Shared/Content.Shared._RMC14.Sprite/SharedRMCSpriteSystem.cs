using System;
using System.Numerics;
using Content.Shared._RMC14.Hands;
using Content.Shared.DrawDepth;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared._RMC14.Sprite;

public abstract class SharedRMCSpriteSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SpriteSetRenderOrderComponent, MapInitEvent>((EntityEventRefHandler<SpriteSetRenderOrderComponent, MapInitEvent>)OnSetRenderOrderMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpriteSetRenderOrderComponent, ItemPickedUpEvent>((EntityEventRefHandler<SpriteSetRenderOrderComponent, ItemPickedUpEvent>)OnSetRenderOrderPickedUp, (Type[])null, (Type[])null);
	}

	private void OnSetRenderOrderMapInit(Entity<SpriteSetRenderOrderComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.RenderOrder.HasValue)
		{
			_appearance.SetData(Entity<SpriteSetRenderOrderComponent>.op_Implicit(ent), (Enum)SpriteSetRenderOrderComponent.Appearance.Key, (object)ent.Comp.RenderOrder, (AppearanceComponent)null);
		}
		if (ent.Comp.Offset.HasValue)
		{
			_appearance.SetData(Entity<SpriteSetRenderOrderComponent>.op_Implicit(ent), (Enum)SpriteSetRenderOrderComponent.Appearance.Offset, (object)ent.Comp.Offset, (AppearanceComponent)null);
		}
	}

	private void OnSetRenderOrderPickedUp(Entity<SpriteSetRenderOrderComponent> ent, ref ItemPickedUpEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Offset.HasValue && !(ent.Comp.Offset == Vector2.Zero))
		{
			ent.Comp.Offset = Vector2.Zero;
			((EntitySystem)this).Dirty<SpriteSetRenderOrderComponent>(ent, (MetaDataComponent)null);
		}
	}

	public void SetOffset(EntityUid ent, Vector2 offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		SpriteSetRenderOrderComponent sprite = ((EntitySystem)this).EnsureComp<SpriteSetRenderOrderComponent>(ent);
		sprite.Offset = offset;
		((EntitySystem)this).Dirty(ent, (IComponent)(object)sprite, (MetaDataComponent)null);
	}

	public void SetRenderOrder(EntityUid ent, int order)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		SpriteSetRenderOrderComponent sprite = ((EntitySystem)this).EnsureComp<SpriteSetRenderOrderComponent>(ent);
		sprite.RenderOrder = order;
		((EntitySystem)this).Dirty(ent, (IComponent)(object)sprite, (MetaDataComponent)null);
	}

	public void SetColor(Entity<SpriteColorComponent?> ent, Color color)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp = ((EntitySystem)this).EnsureComp<SpriteColorComponent>(Entity<SpriteColorComponent>.op_Implicit(ent));
		ent.Comp.Color = color;
		((EntitySystem)this).Dirty<SpriteColorComponent>(ent, (MetaDataComponent)null);
	}

	public Content.Shared.DrawDepth.DrawDepth GetDrawDepth(EntityUid ent, Content.Shared.DrawDepth.DrawDepth current = Content.Shared.DrawDepth.DrawDepth.Mobs)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GetDrawDepthEvent ev = new GetDrawDepthEvent(current);
		((EntitySystem)this).RaiseLocalEvent<GetDrawDepthEvent>(ent, ref ev, false);
		return ev.DrawDepth;
	}

	public virtual Content.Shared.DrawDepth.DrawDepth UpdateDrawDepth(EntityUid sprite)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Content.Shared.DrawDepth.DrawDepth depth = GetDrawDepth(sprite);
		_appearance.SetData(sprite, (Enum)RMCSpriteDrawDepth.Key, (object)depth, (AppearanceComponent)null);
		return depth;
	}
}
