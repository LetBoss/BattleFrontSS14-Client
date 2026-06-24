using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;

namespace Content.Client.Actions;

[ByRefEvent]
public record struct ActionTargetAttemptEvent(PointerInputCmdArgs Input, Entity<ActionsComponent> User, ActionComponent Action, bool Handled = false, bool FoundTarget = false);
