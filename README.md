# Unluau ![Workflow](https://github.com/valencefun/unluau/actions/workflows/dotnet.yml/badge.svg) [![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

A decompiler for Roblox's Lua based programming language (Luau https://luau-lang.org/). The Unluau project consists of two different .NET namespaces: ``Unluau`` (the main decompiler) and ``Unluau.CLI`` the command line interface for the decompiler. As of right now, Unluau is the most powerful Luau decompiler availible to the public, supporting most of Luau's opcodes and language features. With Unluau you can take a compiled Luau script and restore it to an accurate representation of what the original script may have been.

Unluau is still in early alpha and is receiving updates frequently. You can download and run any of the alpha versions in the [releases](https://github.com/valencefun/UnluauNET/releases) tab.

> **Disclamer:** Unluau is still in very early alpha so the decompiler is not flawless. There will most definitely be bugs that you will encounter when decompiling complex scripts. Forloops are currently not supported, but are coming in future versions.

## Usage
After downloading the latest version in the [releases](https://github.com/valencefun/UnluauNET/releases) tab, you can refer to the manuals below to familiarize yourself with the different binaries availible to you.
* [Unluau CLI](docs/cli.md)
