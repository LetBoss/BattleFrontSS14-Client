using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Stealth.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedStealthSystem) })]
public sealed class StealthComponent : Component, ISerializationGenerated<StealthComponent>, ISerializationGenerated
{
	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField("enabledOnDeath", false, 1, false, false, null)]
	public bool EnabledOnDeath = true;

	[DataField("hadOutline", false, 1, false, false, null)]
	public bool HadOutline;

	[DataField("examineThreshold", false, 1, false, false, null)]
	public float ExamineThreshold = 0.5f;

	[DataField("lastVisibility", false, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public float LastVisibility = 1f;

	[DataField("lastUpdate", false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan? LastUpdated;

	[DataField("minVisibility", false, 1, false, false, null)]
	public float MinVisibility = -1f;

	[DataField("maxVisibility", false, 1, false, false, null)]
	public float MaxVisibility = 1.5f;

	[DataField("examinedDesc", false, 1, false, false, null)]
	public string ExaminedDesc = "stealth-visual-effect";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StealthComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StealthComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StealthComponent>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
			bool EnabledOnDeathTemp = false;
			if (!serialization.TryCustomCopy<bool>(EnabledOnDeath, ref EnabledOnDeathTemp, hookCtx, false, context))
			{
				EnabledOnDeathTemp = EnabledOnDeath;
			}
			target.EnabledOnDeath = EnabledOnDeathTemp;
			bool HadOutlineTemp = false;
			if (!serialization.TryCustomCopy<bool>(HadOutline, ref HadOutlineTemp, hookCtx, false, context))
			{
				HadOutlineTemp = HadOutline;
			}
			target.HadOutline = HadOutlineTemp;
			float ExamineThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ExamineThreshold, ref ExamineThresholdTemp, hookCtx, false, context))
			{
				ExamineThresholdTemp = ExamineThreshold;
			}
			target.ExamineThreshold = ExamineThresholdTemp;
			float LastVisibilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LastVisibility, ref LastVisibilityTemp, hookCtx, false, context))
			{
				LastVisibilityTemp = LastVisibility;
			}
			target.LastVisibility = LastVisibilityTemp;
			TimeSpan? LastUpdatedTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(LastUpdated, ref LastUpdatedTemp, hookCtx, false, context))
			{
				LastUpdatedTemp = serialization.CreateCopy<TimeSpan?>(LastUpdated, hookCtx, context, false);
			}
			target.LastUpdated = LastUpdatedTemp;
			float MinVisibilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinVisibility, ref MinVisibilityTemp, hookCtx, false, context))
			{
				MinVisibilityTemp = MinVisibility;
			}
			target.MinVisibility = MinVisibilityTemp;
			float MaxVisibilityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxVisibility, ref MaxVisibilityTemp, hookCtx, false, context))
			{
				MaxVisibilityTemp = MaxVisibility;
			}
			target.MaxVisibility = MaxVisibilityTemp;
			string ExaminedDescTemp = null;
			if (ExaminedDesc == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ExaminedDesc, ref ExaminedDescTemp, hookCtx, false, context))
			{
				ExaminedDescTemp = ExaminedDesc;
			}
			target.ExaminedDesc = ExaminedDescTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StealthComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealthComponent cast = (StealthComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealthComponent cast = (StealthComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealthComponent def = (StealthComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StealthComponent Instantiate()
	{
		return new StealthComponent();
	}
}
