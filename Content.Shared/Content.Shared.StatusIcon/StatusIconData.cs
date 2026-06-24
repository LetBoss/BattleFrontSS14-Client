using System;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.StatusIcon;

[Virtual]
[DataDefinition]
public class StatusIconData : IComparable<StatusIconData>, ISerializationGenerated<StatusIconData>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public SpriteSpecifier Icon;

	[DataField(null, false, 1, false, false, null)]
	public int Priority = 10;

	[DataField(null, false, 1, false, false, null)]
	public bool VisibleToGhosts = true;

	[DataField(null, false, 1, false, false, null)]
	public bool HideInContainer = true;

	[DataField(null, false, 1, false, false, null)]
	public bool HideOnStealth = true;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? ShowTo;

	[DataField(null, false, 1, false, false, null)]
	public StatusIconLocationPreference LocationPreference;

	[DataField(null, false, 1, false, false, null)]
	public StatusIconLayer Layer;

	[DataField(null, false, 1, false, false, null)]
	public int Offset;

	[DataField(null, false, 1, false, false, null)]
	public bool IsShaded;

	public int CompareTo(StatusIconData? other)
	{
		return Priority.CompareTo(other?.Priority ?? int.MaxValue);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref StatusIconData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<StatusIconData>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		SpriteSpecifier IconTemp = null;
		if (Icon == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SpriteSpecifier>(Icon, ref IconTemp, hookCtx, true, context))
		{
			IconTemp = serialization.CreateCopy<SpriteSpecifier>(Icon, hookCtx, context, false);
		}
		target.Icon = IconTemp;
		int PriorityTemp = 0;
		if (!serialization.TryCustomCopy<int>(Priority, ref PriorityTemp, hookCtx, false, context))
		{
			PriorityTemp = Priority;
		}
		target.Priority = PriorityTemp;
		bool VisibleToGhostsTemp = false;
		if (!serialization.TryCustomCopy<bool>(VisibleToGhosts, ref VisibleToGhostsTemp, hookCtx, false, context))
		{
			VisibleToGhostsTemp = VisibleToGhosts;
		}
		target.VisibleToGhosts = VisibleToGhostsTemp;
		bool HideInContainerTemp = false;
		if (!serialization.TryCustomCopy<bool>(HideInContainer, ref HideInContainerTemp, hookCtx, false, context))
		{
			HideInContainerTemp = HideInContainer;
		}
		target.HideInContainer = HideInContainerTemp;
		bool HideOnStealthTemp = false;
		if (!serialization.TryCustomCopy<bool>(HideOnStealth, ref HideOnStealthTemp, hookCtx, false, context))
		{
			HideOnStealthTemp = HideOnStealth;
		}
		target.HideOnStealth = HideOnStealthTemp;
		EntityWhitelist ShowToTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(ShowTo, ref ShowToTemp, hookCtx, false, context))
		{
			if (ShowTo == null)
			{
				ShowToTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(ShowTo, ref ShowToTemp, hookCtx, context, false);
			}
		}
		target.ShowTo = ShowToTemp;
		StatusIconLocationPreference LocationPreferenceTemp = StatusIconLocationPreference.None;
		if (!serialization.TryCustomCopy<StatusIconLocationPreference>(LocationPreference, ref LocationPreferenceTemp, hookCtx, false, context))
		{
			LocationPreferenceTemp = LocationPreference;
		}
		target.LocationPreference = LocationPreferenceTemp;
		StatusIconLayer LayerTemp = StatusIconLayer.Base;
		if (!serialization.TryCustomCopy<StatusIconLayer>(Layer, ref LayerTemp, hookCtx, false, context))
		{
			LayerTemp = Layer;
		}
		target.Layer = LayerTemp;
		int OffsetTemp = 0;
		if (!serialization.TryCustomCopy<int>(Offset, ref OffsetTemp, hookCtx, false, context))
		{
			OffsetTemp = Offset;
		}
		target.Offset = OffsetTemp;
		bool IsShadedTemp = false;
		if (!serialization.TryCustomCopy<bool>(IsShaded, ref IsShadedTemp, hookCtx, false, context))
		{
			IsShadedTemp = IsShaded;
		}
		target.IsShaded = IsShadedTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref StatusIconData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StatusIconData cast = (StatusIconData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual StatusIconData Instantiate()
	{
		return new StatusIconData();
	}
}
