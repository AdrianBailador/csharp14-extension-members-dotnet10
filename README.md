# C# 14 Extension Members

Companion repo for [C# 14 Extension Members: The Shadowing Bug the Compiler Won't Warn You About](https://adrianbailador.github.io/blog/76-csharp14-extension-members) — a runnable demo of the full C# 14 `extension` block syntax, and the two compiler diagnostics (CS9296, CS9339) and one silent runtime behavior that the article is actually about.

## The claims, verified

Every claim in the article is backed by a test, not just quoted from documentation:

- **`ExtensionUsageTests`** — the full syntax surface works as documented: instance extension property, instance extension method, static extension member called as `Order.Empty(...)`.
- **`ShadowingTests`** — when a real instance member and an extension member share a name, the real member wins. Silently. `dotnet build` on this repo produces **0 warnings**, even though `OrderV2Extensions.IsEligibleForFreeShipping` is permanently unreachable dead code the moment `OrderV2` declares its own member with that name.
- **`CompilerDiagnosticsTests`** — compiles three snippets in-memory with Roslyn and asserts on the exact diagnostics: a clean extension block compiles with zero errors, an extension property referenced inside an `Expression<Func<T,bool>>` fails with **CS9296**, and two extension blocks declaring the same member name for the same receiver fail with **CS9339**.

## Running it

```bash
dotnet test     # 8 tests, 0 skipped
dotnet build    # 0 warnings — including the shadowed OrderV2Extensions member
```

## Project layout

```
src/ExtensionMembers.Domain/
  Order.cs                 the "before" type — a plain record, no domain behavior
  OrderExtensions.cs       the full extension surface: instance property, instance
                            method, static factory member
  OrderV2.cs                OrderV2 (real member) + OrderV2Extensions (stale extension
                            with the same name) — the shadowing scenario
tests/ExtensionMembers.Tests/
  ExtensionUsageTests.cs        normal usage of every extension member kind
  ShadowingTests.cs              pins the real-member-wins behavior with a regression test
  CompilerDiagnosticsTests.cs    Roslyn-based: asserts CS9296 and CS9339 by compiling
                                  the exact failing snippets in-memory
```

## License

MIT — use this however's useful.
