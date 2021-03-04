# Setting Up

You need to install:
- Visual Studio Code (or Visual Studio)
- .Net Core SDK 3.1
- Monogame (at least the MGCB editor)
- Tiled (for editing maps)

You should also install the C# plugin for vs code to get intellisense and other goodies.

# Building Assets
Monogame uses a content pipeline for The Most Optimal performance. This packs all the images, sounds and maps into a single file that the game reads.

You need to rebuild the content every time you edit a map or image etc.

- Run the Monogame MGCB Editor
- Open `Content/Content.mgcb`
- Press build

# Running
- Run `dotnet restore` to pull deps
- Run `dotnet run` to run

# Debugging
The repository comes with a .vscode directory that should automatically configure the vscode debugger, just press play.
