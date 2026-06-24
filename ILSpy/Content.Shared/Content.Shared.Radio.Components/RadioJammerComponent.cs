using System;
using System.ComponentModel;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Radio.Components;

[NetworkedComponent]
[RegisterComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RadioJammerComponent : Component, ISerializationGenerated<RadioJammerComponent>, ISerializationGenerated
{
	[DataDefinition]
	public struct RadioJamSetting : ISerializationGenerated<RadioJamSetting>, ISerializationGenerated
	{
		[DataField(null, false, 1, true, false, null)]
		public float Wattage = 0f;

		[DataField(null, false, 1, true, false, null)]
		public float Range = 0f;

		[DataField(null, false, 1, true, false, null)]
		public LocId Message = LocId.op_Implicit(string.Empty);

		[DataField(null, false, 1, true, false, null)]
		public LocId Name = LocId.op_Implicit(string.Empty);

		public RadioJamSetting()
		{
		}//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)


		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref RadioJamSetting target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			if (!serialization.TryCustomCopy<RadioJamSetting>(this, ref target, hookCtx, false, context))
			{
				float WattageTemp = 0f;
				if (!serialization.TryCustomCopy<float>(Wattage, ref WattageTemp, hookCtx, false, context))
				{
					WattageTemp = Wattage;
				}
				float RangeTemp = 0f;
				if (!serialization.TryCustomCopy<float>(Range, ref RangeTemp, hookCtx, false, context))
				{
					RangeTemp = Range;
				}
				LocId MessageTemp = default(LocId);
				if (!serialization.TryCustomCopy<LocId>(Message, ref MessageTemp, hookCtx, false, context))
				{
					MessageTemp = serialization.CreateCopy<LocId>(Message, hookCtx, context, false);
				}
				LocId NameTemp = default(LocId);
				if (!serialization.TryCustomCopy<LocId>(Name, ref NameTemp, hookCtx, false, context))
				{
					NameTemp = serialization.CreateCopy<LocId>(Name, hookCtx, context, false);
				}
				RadioJamSetting radioJamSetting = target;
				radioJamSetting.Wattage = WattageTemp;
				radioJamSetting.Range = RangeTemp;
				radioJamSetting.Message = MessageTemp;
				radioJamSetting.Name = NameTemp;
				target = radioJamSetting;
			}
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref RadioJamSetting target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			RadioJamSetting cast = (RadioJamSetting)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public RadioJamSetting Instantiate()
		{
			return new RadioJamSetting();
		}
	}

	[Serializable]
	[NetSerializable]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class RadioJammerComponent_AutoState : IComponentState
	{
		public int SelectedPowerLevel;
	}

	[RobustAutoGenerated]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class RadioJammerComponent_AutoNetworkSystem : EntitySystem
	{
		public override void Initialize()
		{
			((EntitySystem)this).SubscribeLocalEvent<RadioJammerComponent, ComponentGetState>((ComponentEventRefHandler<RadioJammerComponent, ComponentGetState>)OnGetState, (Type[])null, (Type[])null);
			((EntitySystem)this).SubscribeLocalEvent<RadioJammerComponent, ComponentHandleState>((ComponentEventRefHandler<RadioJammerComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		}

		private void OnGetState(EntityUid uid, RadioJammerComponent component, ref ComponentGetState args)
		{
			((ComponentGetState)(ref args)).State = (IComponentState)(object)new RadioJammerComponent_AutoState
			{
				SelectedPowerLevel = component.SelectedPowerLevel
			};
		}

		private void OnHandleState(EntityUid uid, RadioJammerComponent component, ref ComponentHandleState args)
		{
			if (((ComponentHandleState)(ref args)).Current is RadioJammerComponent_AutoState state)
			{
				component.SelectedPowerLevel = state.SelectedPowerLevel;
			}
		}
	}

	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public RadioJamSetting[] Settings;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public int SelectedPowerLevel = 1;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RadioJammerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RadioJammerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RadioJammerComponent>(this, ref target, hookCtx, false, context))
		{
			RadioJamSetting[] SettingsTemp = null;
			if (Settings == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<RadioJamSetting[]>(Settings, ref SettingsTemp, hookCtx, true, context))
			{
				SettingsTemp = serialization.CreateCopy<RadioJamSetting[]>(Settings, hookCtx, context, false);
			}
			target.Settings = SettingsTemp;
			int SelectedPowerLevelTemp = 0;
			if (!serialization.TryCustomCopy<int>(SelectedPowerLevel, ref SelectedPowerLevelTemp, hookCtx, false, context))
			{
				SelectedPowerLevelTemp = SelectedPowerLevel;
			}
			target.SelectedPowerLevel = SelectedPowerLevelTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RadioJammerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadioJammerComponent cast = (RadioJammerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadioJammerComponent cast = (RadioJammerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadioJammerComponent def = (RadioJammerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RadioJammerComponent Instantiate()
	{
		return new RadioJammerComponent();
	}
}
