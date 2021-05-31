# PBXEdit
Lightweight Xcode project editor

![PBXEdit](https://github.com/ZehMatt/PBXEdit/blob/master/.github/PBXEdit.png?raw=true)

# Description
A simple Xcode project editor to quickly add or remove files without the need of Xcode. All of the
PBX serialization/deserialization is based on studying one of the project files from a real project,
some things might be wrong but it seems to good enough to keep the build pipeline happy.

# Why
The reason is quite simple, we (OpenRCT2) needed a way to manage the Xcode project without
having to use Xcode which is only available for MacOS. You may or may not find this helpful
for your own projects. The UI and serializer/deserializer are separate, you can write CI tools
if you want to.

# Bugs
Probably has some, if you encounter one please create a new issue, I may or may not fix it,
PR's are always welcome.
