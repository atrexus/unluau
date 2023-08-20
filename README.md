# Unluau
A decompiler for Roblox's Lua based programming language (Luau https://luau-lang.org/). The Unluau project consists of two different .NET namespaces: ``Unluau`` (the main decompiler) and ``Unluau.CLI`` the command line interface for the decompiler. As of right now, Unluau is the most powerful Luau decompiler availible to the public, supporting most of Luau's opcodes and language features. With Unluau you can take a compiled Luau script and restore it to an accurate representation of what the original script may have been.

Unluau is still in early alpha and is receiving updates frequently. You can download and run any of the alpha versions in the [releases](https://github.com/societall/UnluauNET/releases) tab.

## Usage
Currently, you can only use the CLI (command line interface) to decompile bytecode. You can see how to use it [here](docs/cli.md).
  
## Benchmarking
To ensure Unluau supports as many Luau language features possible we have created a test suite of various scripts that must pass after every commit to the main branch. These tests can be viewed [here](Unluau.Test/Expect).

If you want to test some of your own scripts but don't know how to generate luau binaries, check out [luau-workspace](https://github.com/valencefun/luau-workspace).
