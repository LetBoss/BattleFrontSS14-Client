using System;
using System.Collections.Generic;
using Content.Shared.Paper;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Fax.Components;

[DataDefinition]
public sealed class FaxPrintout : ISerializationGenerated<FaxPrintout>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Name { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string? Label { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	public string Content { get; private set; }

	[DataField(null, false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string PrototypeId { get; private set; }

	[DataField("stampState", false, 1, false, false, null)]
	public string? StampState { get; private set; }

	[DataField("stampedBy", false, 1, false, false, null)]
	public List<StampDisplayInfo> StampedBy { get; private set; } = new List<StampDisplayInfo>();

	[DataField(null, false, 1, false, false, null)]
	public bool Locked { get; private set; }

	private FaxPrintout()
	{
	}

	public FaxPrintout(string content, string name, string? label = null, string? prototypeId = null, string? stampState = null, List<StampDisplayInfo>? stampedBy = null, bool locked = false)
	{
		Content = content;
		Name = name;
		Label = label;
		PrototypeId = prototypeId ?? "";
		StampState = stampState;
		StampedBy = stampedBy ?? new List<StampDisplayInfo>();
		Locked = locked;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FaxPrintout target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<FaxPrintout>(this, ref target, hookCtx, false, context))
		{
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
			string LabelTemp = null;
			if (!serialization.TryCustomCopy<string>(Label, ref LabelTemp, hookCtx, false, context))
			{
				LabelTemp = Label;
			}
			target.Label = LabelTemp;
			string ContentTemp = null;
			if (Content == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Content, ref ContentTemp, hookCtx, false, context))
			{
				ContentTemp = Content;
			}
			target.Content = ContentTemp;
			string PrototypeIdTemp = null;
			if (PrototypeId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PrototypeId, ref PrototypeIdTemp, hookCtx, false, context))
			{
				PrototypeIdTemp = PrototypeId;
			}
			target.PrototypeId = PrototypeIdTemp;
			string StampStateTemp = null;
			if (!serialization.TryCustomCopy<string>(StampState, ref StampStateTemp, hookCtx, false, context))
			{
				StampStateTemp = StampState;
			}
			target.StampState = StampStateTemp;
			List<StampDisplayInfo> StampedByTemp = null;
			if (StampedBy == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<StampDisplayInfo>>(StampedBy, ref StampedByTemp, hookCtx, true, context))
			{
				StampedByTemp = serialization.CreateCopy<List<StampDisplayInfo>>(StampedBy, hookCtx, context, false);
			}
			target.StampedBy = StampedByTemp;
			bool LockedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Locked, ref LockedTemp, hookCtx, false, context))
			{
				LockedTemp = Locked;
			}
			target.Locked = LockedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FaxPrintout target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FaxPrintout cast = (FaxPrintout)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public FaxPrintout Instantiate()
	{
		return new FaxPrintout();
	}
}
