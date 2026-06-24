using System;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Interactable.Components;

[RegisterComponent]
public sealed class InteractionOutlineComponent : Component, ISerializationGenerated<InteractionOutlineComponent>, ISerializationGenerated
{
	private static readonly ProtoId<ShaderPrototype> ShaderInRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutlineInrange");

	private static readonly ProtoId<ShaderPrototype> ShaderOutOfRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutline");

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IEntityManager _entMan;

	private const float DefaultWidth = 1f;

	private bool _inRange;

	private ShaderInstance? _shader;

	private int _lastRenderScale;

	public void OnMouseEnter(EntityUid uid, bool inInteractionRange, int renderScale)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		_lastRenderScale = renderScale;
		_inRange = inInteractionRange;
		SpriteComponent val = default(SpriteComponent);
		if (_entMan.TryGetComponent<SpriteComponent>(uid, ref val) && val.PostShader == null)
		{
			_shader = MakeNewShader(inInteractionRange, renderScale);
			val.PostShader = _shader;
		}
	}

	public void OnMouseLeave(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (_entMan.TryGetComponent<SpriteComponent>(uid, ref val))
		{
			if (val.PostShader == _shader)
			{
				val.PostShader = null;
			}
			val.RenderOrder = 0u;
		}
		ShaderInstance? shader = _shader;
		if (shader != null)
		{
			shader.Dispose();
		}
		_shader = null;
	}

	public void UpdateInRange(EntityUid uid, bool inInteractionRange, int renderScale)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (_entMan.TryGetComponent<SpriteComponent>(uid, ref val) && val.PostShader == _shader && (inInteractionRange != _inRange || _lastRenderScale != renderScale))
		{
			_inRange = inInteractionRange;
			_lastRenderScale = renderScale;
			_shader = MakeNewShader(_inRange, _lastRenderScale);
			val.PostShader = _shader;
		}
	}

	private ShaderInstance MakeNewShader(bool inRange, int renderScale)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ProtoId<ShaderPrototype> val = (inRange ? ShaderInRange : ShaderOutOfRange);
		ShaderInstance obj = _prototypeManager.Index<ShaderPrototype>(val).InstanceUnique();
		obj.SetParameter("outline_width", 1f * (float)renderScale);
		return obj;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InteractionOutlineComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (InteractionOutlineComponent)(object)val;
		serialization.TryCustomCopy<InteractionOutlineComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InteractionOutlineComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InteractionOutlineComponent target2 = (InteractionOutlineComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InteractionOutlineComponent target2 = (InteractionOutlineComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InteractionOutlineComponent target2 = (InteractionOutlineComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InteractionOutlineComponent Instantiate()
	{
		return new InteractionOutlineComponent();
	}
}
