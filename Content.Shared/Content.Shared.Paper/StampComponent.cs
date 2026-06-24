using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Paper;

[RegisterComponent]
public sealed class StampComponent : Component, ISerializationGenerated<StampComponent>, ISerializationGenerated
{
	[DataField("stampedColor", false, 1, false, false, null)]
	public Color StampedColor = Color.FromHex((ReadOnlySpan<char>)"#BB3232", (Color?)null);

	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier? Sound;

	[DataField("stampedName", false, 1, false, false, null)]
	public string StampedName { get; set; } = "stamp-component-stamped-name-default";

	[DataField("stampState", false, 1, false, false, null)]
	public string StampState { get; set; } = "paper_stamp-generic";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StampComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StampComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StampComponent>(this, ref target, hookCtx, false, context))
		{
			string StampedNameTemp = null;
			if (StampedName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StampedName, ref StampedNameTemp, hookCtx, false, context))
			{
				StampedNameTemp = StampedName;
			}
			target.StampedName = StampedNameTemp;
			string StampStateTemp = null;
			if (StampState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StampState, ref StampStateTemp, hookCtx, false, context))
			{
				StampStateTemp = StampState;
			}
			target.StampState = StampStateTemp;
			Color StampedColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(StampedColor, ref StampedColorTemp, hookCtx, false, context))
			{
				StampedColorTemp = serialization.CreateCopy<Color>(StampedColor, hookCtx, context, false);
			}
			target.StampedColor = StampedColorTemp;
			SoundSpecifier SoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StampComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StampComponent cast = (StampComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StampComponent cast = (StampComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StampComponent def = (StampComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StampComponent Instantiate()
	{
		return new StampComponent();
	}
}
