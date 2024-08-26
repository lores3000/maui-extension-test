# Test project for ios app extension. Does not run in VS Code, runs with command lines:

## Get Device ID:
xcrun xctrace list devices

## Build:
dotnet build /p:RuntimeIdentifier=ios-arm64 /bl -f net8.0-ios

## Run: (use device name from xcrun)
/usr/local/share/dotnet/packs/Microsoft.iOS.Sdk/17.2.8053/tools/bin/mlaunch --installdev bin/Debug/net8.0-ios/ios-arm64/app.app --devname=00008110-000E14590E42801E

## Github issue:
https://github.com/microsoft/vscode-dotnettools/issues/1386