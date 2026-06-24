using System.Collections.Generic;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Medical.Surgery.Steps;

[ByRefEvent]
public record struct CMSurgeryCanPerformStepEvent(EntityUid User, EntityUid Body, List<EntityUid> Tools, SlotFlags TargetSlots, string? Popup = null, StepInvalidReason Invalid = StepInvalidReason.None, HashSet<EntityUid>? ValidTools = null) : IInventoryRelayEvent;
