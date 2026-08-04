# ☩ CrosshairApp

A lightweight crosshair overlay app for Windows. Place a customizable crosshair on top of any game or application.

## Features

- **3 crosshair styles**: Cross, Dot, Circle
- **Full color picker** with preset colors and custom color wheel
- **Adjustable settings**: Size, Thickness, Opacity, Rotation, Gap, RGB animated mode
- **Live preview** in the app window
- **Transparent overlay** that sits on top of any game
- **Launch at Windows startup** option
- **No installation needed** — just double-click the `.exe`

## Download

👉 Go to the [Releases](../../releases) page and download `CrosshairApp.exe`

> **Note for Windows 11 users:** If Smart App Control blocks the file, right-click the `.exe` → Properties → check **Unblock** → OK, then run it.

## Usage

1. Download `CrosshairApp.exe` from Releases
2. Run it
3. Adjust your crosshair style, color, and size in the **Crosshair** tab
4. Click **Toggle Overlay on Screen** to show the crosshair on your desktop/game
5. Go to **Settings** to enable launch at startup

## Building from Source

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
git clone https://github.com/YOUR_USERNAME/CrosshairApp.git
cd CrosshairApp
dotnet run --project CrosshairApp.csproj
```

## License

MIT
