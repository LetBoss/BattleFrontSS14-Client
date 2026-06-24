using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.TacticalMap;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedTacticalMapSystem) })]
public sealed class ActiveTacticalMapTrackedComponent : Component, ISerializationGenerated<ActiveTacticalMapTrackedComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Map;

	[DataField(null, false, 1, false, false, null)]
	public Rsi? Icon;

	[DataField(null, false, 1, false, false, null)]
	public Color Color;

	[DataField(null, false, 1, false, false, null)]
	public bool Undefibbable;

	[DataField(null, false, 1, false, false, null)]
	public Rsi? Background;

	[DataField(null, false, 1, false, false, null)]
	public bool HiveLeader;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ActiveTacticalMapTrackedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ActiveTacticalMapTrackedComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ActiveTacticalMapTrackedComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityUid? MapTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(Map, ref MapTemp, hookCtx, false, context))
		{
			MapTemp = serialization.CreateCopy<EntityUid?>(Map, hookCtx, context, false);
		}
		target.Map = MapTemp;
		Rsi IconTemp = null;
		if (!serialization.TryCustomCopy<Rsi>(Icon, ref IconTemp, hookCtx, false, context))
		{
			if (Icon == null)
			{
				IconTemp = null;
			}
			else
			{
				serialization.CopyTo<Rsi>(Icon, ref IconTemp, hookCtx, context, false);
			}
		}
		target.Icon = IconTemp;
		Color ColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(Color, ref ColorTemp, hookCtx, false, context))
		{
			ColorTemp = serialization.CreateCopy<Color>(Color, hookCtx, context, false);
		}
		target.Color = ColorTemp;
		bool UndefibbableTemp = false;
		if (!serialization.TryCustomCopy<bool>(Undefibbable, ref UndefibbableTemp, hookCtx, false, context))
		{
			UndefibbableTemp = Undefibbable;
		}
		target.Undefibbable = UndefibbableTemp;
		Rsi BackgroundTemp = null;
		if (!serialization.TryCustomCopy<Rsi>(Background, ref BackgroundTemp, hookCtx, false, context))
		{
			if (Background == null)
			{
				BackgroundTemp = null;
			}
			else
			{
				serialization.CopyTo<Rsi>(Background, ref BackgroundTemp, hookCtx, context, false);
			}
		}
		target.Background = BackgroundTemp;
		bool HiveLeaderTemp = false;
		if (!serialization.TryCustomCopy<bool>(HiveLeader, ref HiveLeaderTemp, hookCtx, false, context))
		{
			HiveLeaderTemp = HiveLeader;
		}
		target.HiveLeader = HiveLeaderTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ActiveTacticalMapTrackedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveTacticalMapTrackedComponent cast = (ActiveTacticalMapTrackedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveTacticalMapTrackedComponent cast = (ActiveTacticalMapTrackedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ActiveTacticalMapTrackedComponent def = (ActiveTacticalMapTrackedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ActiveTacticalMapTrackedComponent Instantiate()
	{
		return new ActiveTacticalMapTrackedComponent();
	}
}
