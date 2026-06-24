using System;
using System.Numerics;
using Content.Client.Interactable.Components;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Stealth;

public sealed class StealthSystem : SharedStealthSystem
{
	private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Stealth");

	[Dependency]
	private IPrototypeManager _protoMan;

	[Dependency]
	private SharedTransformSystem _transformSystem;

	[Dependency]
	private SpriteSystem _sprite;

	private ShaderInstance _shader;

	public override void Initialize()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_shader = _protoMan.Index<ShaderPrototype>(Shader).InstanceUnique();
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, ComponentShutdown>((ComponentEventHandler<StealthComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, ComponentStartup>((ComponentEventHandler<StealthComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StealthComponent, BeforePostShaderRenderEvent>((ComponentEventHandler<StealthComponent, BeforePostShaderRenderEvent>)OnShaderRender, (Type[])null, (Type[])null);
	}

	public override void SetEnabled(EntityUid uid, bool value, StealthComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StealthComponent>(uid, ref component, true) && component.Enabled != value)
		{
			base.SetEnabled(uid, value, component);
			SetShader(uid, value, component);
		}
	}

	private void SetShader(EntityUid uid, bool enabled, StealthComponent? component = null, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StealthComponent, SpriteComponent>(uid, ref component, ref sprite, false))
		{
			return;
		}
		_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), Color.White);
		sprite.PostShader = (enabled ? _shader : null);
		sprite.GetScreenTexture = enabled;
		sprite.RaiseShaderEvent = enabled;
		InteractionOutlineComponent interactionOutlineComponent = default(InteractionOutlineComponent);
		if (!enabled)
		{
			if (component.HadOutline && !((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
			{
				((EntitySystem)this).EnsureComp<InteractionOutlineComponent>(uid);
			}
		}
		else if (((EntitySystem)this).TryComp<InteractionOutlineComponent>(uid, ref interactionOutlineComponent))
		{
			((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)interactionOutlineComponent);
			component.HadOutline = true;
		}
	}

	private void OnStartup(EntityUid uid, StealthComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetShader(uid, component.Enabled, component);
	}

	private void OnShutdown(EntityUid uid, StealthComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Terminating(uid, (MetaDataComponent)null))
		{
			SetShader(uid, enabled: false, component);
		}
	}

	private void OnShaderRender(EntityUid uid, StealthComponent component, BeforePostShaderRenderEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parentUid = ((EntitySystem)this).Transform(uid).ParentUid;
		if (((EntityUid)(ref parentUid)).IsValid())
		{
			TransformComponent val = ((EntitySystem)this).Transform(parentUid);
			Vector2 vector = args.Viewport.WorldToLocal(_transformSystem.GetWorldPosition(val));
			vector.X = 0f - vector.X;
			float visibility = GetVisibility(uid, component);
			visibility = Math.Clamp(visibility, -1f, 1f);
			_shader.SetParameter("reference", vector);
			_shader.SetParameter("visibility", visibility);
			visibility = MathF.Max(0f, visibility);
			_sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), new Color(visibility, visibility, 1f, 1f));
		}
	}
}
