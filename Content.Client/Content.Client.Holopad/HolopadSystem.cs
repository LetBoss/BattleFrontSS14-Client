using System;
using System.Linq;
using System.Numerics;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Holopad;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Holopad;

public sealed class HolopadSystem : SharedHolopadSystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HolopadHologramComponent, ComponentStartup>((EntityEventRefHandler<HolopadHologramComponent, ComponentStartup>)OnComponentStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HolopadHologramComponent, BeforePostShaderRenderEvent>((EntityEventRefHandler<HolopadHologramComponent, BeforePostShaderRenderEvent>)OnShaderRender, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<TypingChangedEvent>((EntitySessionEventHandler<TypingChangedEvent>)OnTypingChanged, (Type[])null, (Type[])null);
	}

	private void OnComponentStartup(Entity<HolopadHologramComponent> entity, ref ComponentStartup ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		UpdateHologramSprite(Entity<HolopadHologramComponent>.op_Implicit(entity), entity.Comp.LinkedEntity);
	}

	private void OnShaderRender(Entity<HolopadHologramComponent> entity, ref BeforePostShaderRenderEvent ev)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (ev.Sprite.PostShader != null)
		{
			UpdateHologramSprite(Entity<HolopadHologramComponent>.op_Implicit(entity), entity.Comp.LinkedEntity);
		}
	}

	private void OnTypingChanged(TypingChangedEvent ev, EntitySessionEventArgs args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (((EntitySystem)this).Exists(attachedEntity) && ((EntitySystem)this).HasComp<HolopadUserComponent>(attachedEntity))
		{
			HolopadUserTypingChangedEvent holopadUserTypingChangedEvent = new HolopadUserTypingChangedEvent(((EntitySystem)this).GetNetEntity(attachedEntity.Value, (MetaDataComponent)null), ev.State);
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)holopadUserTypingChangedEvent);
		}
	}

	private void UpdateHologramSprite(EntityUid hologram, EntityUid? target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		HolopadHologramComponent holopadHologramComponent = default(HolopadHologramComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(hologram, ref val) || !((EntitySystem)this).TryComp<HolopadHologramComponent>(hologram, ref holopadHologramComponent))
		{
			return;
		}
		for (int num = val.AllLayers.Count() - 1; num >= 0; num--)
		{
			_sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((hologram, val)), num, true);
		}
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(target, ref item))
		{
			HolographicAvatarComponent holographicAvatarComponent = default(HolographicAvatarComponent);
			if (((EntitySystem)this).TryComp<HolographicAvatarComponent>(target, ref holographicAvatarComponent) && holographicAvatarComponent.LayerData != null)
			{
				for (int i = 0; i < holographicAvatarComponent.LayerData.Length; i++)
				{
					_sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((hologram, val)), holographicAvatarComponent.LayerData[i], (int?)i);
				}
			}
			else
			{
				_sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((target.Value, item)), Entity<SpriteComponent>.op_Implicit((hologram, val)));
			}
		}
		else
		{
			if (string.IsNullOrEmpty(holopadHologramComponent.RsiPath) || string.IsNullOrEmpty(holopadHologramComponent.RsiState))
			{
				return;
			}
			PrototypeLayerData val2 = new PrototypeLayerData
			{
				RsiPath = holopadHologramComponent.RsiPath,
				State = holopadHologramComponent.RsiState
			};
			_sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((hologram, val)), val2, (int?)null);
		}
		_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((hologram, val)), Color.White);
		_sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((hologram, val)), holopadHologramComponent.Offset);
		_sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((hologram, val)), 6);
		val.NoRotation = true;
		val.DirectionOverride = (Direction)0;
		val.EnableDirectionOverride = true;
		Layer val3 = default(Layer);
		for (int j = 0; j < val.AllLayers.Count(); j++)
		{
			if (_sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((hologram, val)), j, ref val3, false))
			{
				ProtoId<ShaderPrototype>? shaderPrototype = val3.ShaderPrototype;
				ProtoId<ShaderPrototype>? val4 = ProtoId<ShaderPrototype>.op_Implicit("DisplacedDraw");
				if (shaderPrototype.HasValue != val4.HasValue || (shaderPrototype.HasValue && shaderPrototype.GetValueOrDefault() != val4.GetValueOrDefault()))
				{
					val.LayerSetShader(j, "unshaded");
				}
			}
		}
		UpdateHologramShader(hologram, val, holopadHologramComponent);
	}

	private void UpdateHologramShader(EntityUid uid, SpriteComponent sprite, HolopadHologramComponent holopadHologram)
	{
		float num = sprite.AllLayers.Max((ISpriteLayer x) => x.PixelSize.Y);
		ShaderInstance val = _prototypeManager.Index<ShaderPrototype>(holopadHologram.ShaderName).InstanceUnique();
		val.SetParameter("color1", new Vector3(holopadHologram.Color1.R, holopadHologram.Color1.G, holopadHologram.Color1.B));
		val.SetParameter("color2", new Vector3(holopadHologram.Color2.R, holopadHologram.Color2.G, holopadHologram.Color2.B));
		val.SetParameter("alpha", holopadHologram.Alpha);
		val.SetParameter("intensity", holopadHologram.Intensity);
		val.SetParameter("texHeight", num);
		val.SetParameter("t", (float)_timing.CurTime.TotalSeconds * holopadHologram.ScrollRate);
		sprite.PostShader = val;
		sprite.RaiseShaderEvent = true;
	}
}
