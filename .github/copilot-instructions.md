# DatastarExamples Copilot Instructions

## Build and test commands

- Preferred repo-level validation from the repository root:
  - `dotnet build DatastarExamples.sln`
  - `dotnet test DatastarExamples.sln --no-build`
- CI uses the same solution and runs Release builds:
  - `dotnet restore DatastarExamples.sln`
  - `dotnet build DatastarExamples.sln --configuration Release --no-restore`
  - `dotnet test DatastarExamples.sln --configuration Release --no-build`
  - `dotnet publish CSharp/Mvc/Mvc.csproj --configuration Release --no-build`
  - `dotnet publish CSharp/MinimalApi/MinimalAPI.csproj --configuration Release --no-build`
  - `dotnet publish CSharp/RazorPages/RazorPages.csproj --configuration Release --no-build`
  - `dotnet publish CSharp/RazorSlices/RazorSlicesExample.csproj --configuration Release --no-build`
- There are currently no committed test projects or checked-in Playwright specs, so there is no repository-defined single-test command yet.

## High-level architecture

- This repository is a side-by-side comparison of the same Datastar examples implemented in four ASP.NET Core styles:
  - `CSharp/Mvc`: controller actions plus Razor views.
  - `CSharp/RazorPages`: page-model plus `.cshtml` pairs.
  - `CSharp/MinimalApi`: route handlers and inline HTML generation in `Program.cs`.
  - `CSharp/RazorSlices`: route handlers that render RazorSlice components and partial slices.
- `DatastarExamples.sln` is the top-level solution that includes all four apps. `CSharp/CSharp.sln` is a nested solution for the C# apps only.
- `Theme/` is the static HTML reference set for the Datastar demos. When implementing or fixing an example, compare the ASP.NET version to the matching theme file before inventing new behavior.
- `PRD.md` is the cross-framework implementation matrix. Use it to check whether an example is supposed to exist in a given app before adding routes or views, and update it when an example's status changes.

## Key conventions

- Example names, routes, and UI navigation stay aligned across frameworks. Example pages use names like `ActiveSearch`, `BulkUpdate`, and `ClickToEdit`, while the public routes use kebab-case such as `/active-search` and `/bulk-update`.
- Datastar server responses are built around Server-Sent Events helpers. Each app has a `Helpers/SseHelper.cs` with shared event names and patch conventions, so reuse those helpers instead of hand-rolling SSE output.
- The implementations share feature intent but not the same state-management strategy:
  - MVC, Razor Pages, and Minimal API frequently keep demo state in static in-memory collections.
  - RazorSlices moves demo state into singleton repositories such as `NoteRepository` and `UserRepository`.
- RazorSlices-specific request parsing lives in `CSharp/RazorSlices/Helpers/DatastarPayloadReader.cs`. Reuse it for Datastar payloads instead of duplicating JSON parsing in new endpoints.
- When adding or finishing an example, keep the surrounding status surfaces in sync: update the relevant home/example navigation partials (`_MoreExamples`) and `PRD.md`, not just the endpoint or page itself.
