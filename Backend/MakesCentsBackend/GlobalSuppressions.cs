// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Style",
    "IDE0290:Use primary constructor",
    Justification = "Keep non-primary constructor for readability and scalability purposes"
)]
[assembly: SuppressMessage("Style", "IDE0270:Use coalesce expression", Justification = "Keep for clarity purposes")]
[assembly: SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Keep for clarity purposes")]
[assembly: SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Keep for clarity purposes")]
[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Keep for clarity purposes")]
[assembly: SuppressMessage("Style", "IDE0037:Use inferred member name", Justification = "Keep for clarity purposes")]
