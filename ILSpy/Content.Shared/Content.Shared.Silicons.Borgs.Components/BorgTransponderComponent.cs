using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Utility;

namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedBorgSystem) })]
public sealed class BorgTransponderComponent : Component, ISerializationGenerated<BorgTransponderComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public SpriteSpecifier? Sprite;

	[DataField(null, false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public LocId DisabledPopup = LocId.op_Implicit("borg-transponder-disabled-popup");

	[DataField(null, false, 1, false, false, null)]
	public LocId DisablingPopup = LocId.op_Implicit("borg-transponder-disabling-popup");

	[DataField(null, false, 1, false, false, null)]
	public LocId DestroyingPopup = LocId.op_Implicit("borg-transponder-destroying-popup");

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan BroadcastDelay = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan NextBroadcast = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan? NextDisable;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DisableDelay = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, null)]
	public bool FakeDisabling;

	[DataField(null, false, 1, false, false, null)]
	public bool FakeDisabled;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BorgTransponderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (BorgTransponderComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BorgTransponderComponent>(this, ref target, hookCtx, false, context))
		{
			SpriteSpecifier SpriteTemp = null;
			if (!serialization.TryCustomCopy<SpriteSpecifier>(Sprite, ref SpriteTemp, hookCtx, true, context))
			{
				SpriteTemp = serialization.CreateCopy<SpriteSpecifier>(Sprite, hookCtx, context, false);
			}
			target.Sprite = SpriteTemp;
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			LocId DisabledPopupTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(DisabledPopup, ref DisabledPopupTemp, hookCtx, false, context))
			{
				DisabledPopupTemp = serialization.CreateCopy<LocId>(DisabledPopup, hookCtx, context, false);
			}
			target.DisabledPopup = DisabledPopupTemp;
			LocId DisablingPopupTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(DisablingPopup, ref DisablingPopupTemp, hookCtx, false, context))
			{
				DisablingPopupTemp = serialization.CreateCopy<LocId>(DisablingPopup, hookCtx, context, false);
			}
			target.DisablingPopup = DisablingPopupTemp;
			LocId DestroyingPopupTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(DestroyingPopup, ref DestroyingPopupTemp, hookCtx, false, context))
			{
				DestroyingPopupTemp = serialization.CreateCopy<LocId>(DestroyingPopup, hookCtx, context, false);
			}
			target.DestroyingPopup = DestroyingPopupTemp;
			TimeSpan BroadcastDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(BroadcastDelay, ref BroadcastDelayTemp, hookCtx, false, context))
			{
				BroadcastDelayTemp = serialization.CreateCopy<TimeSpan>(BroadcastDelay, hookCtx, context, false);
			}
			target.BroadcastDelay = BroadcastDelayTemp;
			TimeSpan NextBroadcastTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(NextBroadcast, ref NextBroadcastTemp, hookCtx, false, context))
			{
				NextBroadcastTemp = serialization.CreateCopy<TimeSpan>(NextBroadcast, hookCtx, context, false);
			}
			target.NextBroadcast = NextBroadcastTemp;
			TimeSpan? NextDisableTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(NextDisable, ref NextDisableTemp, hookCtx, false, context))
			{
				NextDisableTemp = serialization.CreateCopy<TimeSpan?>(NextDisable, hookCtx, context, false);
			}
			target.NextDisable = NextDisableTemp;
			TimeSpan DisableDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DisableDelay, ref DisableDelayTemp, hookCtx, false, context))
			{
				DisableDelayTemp = serialization.CreateCopy<TimeSpan>(DisableDelay, hookCtx, context, false);
			}
			target.DisableDelay = DisableDelayTemp;
			bool FakeDisablingTemp = false;
			if (!serialization.TryCustomCopy<bool>(FakeDisabling, ref FakeDisablingTemp, hookCtx, false, context))
			{
				FakeDisablingTemp = FakeDisabling;
			}
			target.FakeDisabling = FakeDisablingTemp;
			bool FakeDisabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(FakeDisabled, ref FakeDisabledTemp, hookCtx, false, context))
			{
				FakeDisabledTemp = FakeDisabled;
			}
			target.FakeDisabled = FakeDisabledTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BorgTransponderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BorgTransponderComponent cast = (BorgTransponderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BorgTransponderComponent cast = (BorgTransponderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BorgTransponderComponent def = (BorgTransponderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BorgTransponderComponent Instantiate()
	{
		return new BorgTransponderComponent();
	}
}
