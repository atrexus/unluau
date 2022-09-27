# UnluauNET
A decompiler for Roblox's Lua based programming language (Luau https://luau-lang.org/). The Unluau project consists of three different .NET namespaces: ``Unluau`` (the main decompiler), ``Unluau.CLI`` the command line interface for the decompiler, and ``Unluau.App`` the windows dissasembler. This project is still under heavy development, so expect many changes.

### What is this?
As of now, this project is a simple decompiler and dissasembler for Luau bytecode. My hope is to make this a completely external tool that has the ability to display all running Roblox scripts in a specific game. This would make it much easier for Lua developers to understand how certain games function and possibly even implement similar stratagies into their own programs/games. 

### How does it work?
The decompiler undergoes three different stages during one decompilation pass. The first one is called the *deserializer*, which simply takes raw bytecode and creates a structure containing the data in a more readable format. Next we have the *lifter* that literally lifts these structures into an **AST** (abstract syntax tree). This tree is then processed and converted to a script (this is out third pass).

## How can I use this?
Currently, you can only use the CLI (command line interface) to decompile bytecode. But, if that is what you are here for, you can run the following command to decompile any bytecode file of your liking:

```
Unluau.CLI.exe  <inputfile.luau>
```
<br>

There are also a variety of command line arguments available for you to use for customization:
```
  --dissasemble          (Default: false) Converts the bytecode to a readable list of instructions.

  -w, --watermark        (Default: false) Displays a watermark comment at the beginning of the decompiled script.

  -v, --verbose          (Default: false) Adds comments above each statement about the instructions it originated from.

  -i, --inline           (Default: false) Inlines table definitions.

  -o, --output           The file that the decompiled script will be stored in (stdout otherwise).

  --help                 Display this help screen.

  --version              Display version information.
  ```
  
  ### Can I contribute?
  Please contribute if you can! My goal was to make this an open source project, so if you have any changes that you would like to implement, feel free to send a pull request and I will review it ASAP.
