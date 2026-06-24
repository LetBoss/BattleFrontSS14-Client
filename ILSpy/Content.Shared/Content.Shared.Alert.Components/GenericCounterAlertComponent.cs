using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Alert.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class GenericCounterAlertComponent : Component, ISerializationGenerated<GenericCounterAlertComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int GlyphWidth = 6;

	[DataField(null, false, 1, false, false, null)]
	public bool CenterGlyph = true;

	[DataField(null, false, 1, false, false, null)]
	public bool HideLeadingZeroes = true;

	[DataField(null, false, 1, false, false, null)]
	public Vector2i AlertSize = new Vector2i(32, 32);

	[DataField(null, false, 1, false, false, null)]
	public List<string> DigitKeys = new List<string> { "1", "10", "100", "1000", "10000" };

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GenericCounterAlertComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GenericCounterAlertComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GenericCounterAlertComponent>(this, ref target, hookCtx, false, context))
		{
			int GlyphWidthTemp = 0;
			if (!serialization.TryCustomCopy<int>(GlyphWidth, ref GlyphWidthTemp, hookCtx, false, context))
			{
				GlyphWidthTemp = GlyphWidth;
			}
			target.GlyphWidth = GlyphWidthTemp;
			bool CenterGlyphTemp = false;
			if (!serialization.TryCustomCopy<bool>(CenterGlyph, ref CenterGlyphTemp, hookCtx, false, context))
			{
				CenterGlyphTemp = CenterGlyph;
			}
			target.CenterGlyph = CenterGlyphTemp;
			bool HideLeadingZeroesTemp = false;
			if (!serialization.TryCustomCopy<bool>(HideLeadingZeroes, ref HideLeadingZeroesTemp, hookCtx, false, context))
			{
				HideLeadingZeroesTemp = HideLeadingZeroes;
			}
			target.HideLeadingZeroes = HideLeadingZeroesTemp;
			Vector2i AlertSizeTemp = default(Vector2i);
			if (!serialization.TryCustomCopy<Vector2i>(AlertSize, ref AlertSizeTemp, hookCtx, false, context))
			{
				AlertSizeTemp = serialization.CreateCopy<Vector2i>(AlertSize, hookCtx, context, false);
			}
			target.AlertSize = AlertSizeTemp;
			List<string> DigitKeysTemp = null;
			if (DigitKeys == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(DigitKeys, ref DigitKeysTemp, hookCtx, true, context))
			{
				DigitKeysTemp = serialization.CreateCopy<List<string>>(DigitKeys, hookCtx, context, false);
			}
			target.DigitKeys = DigitKeysTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GenericCounterAlertComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericCounterAlertComponent cast = (GenericCounterAlertComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericCounterAlertComponent cast = (GenericCounterAlertComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GenericCounterAlertComponent def = (GenericCounterAlertComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GenericCounterAlertComponent Instantiate()
	{
		return new GenericCounterAlertComponent();
	}
}
