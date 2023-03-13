# UnluauNET
A decompiler for Roblox's Lua based programming language (Luau https://luau-lang.org/). The Unluau project consists of three different .NET namespaces: ``Unluau`` (the main decompiler) and ``Unluau.CLI`` the command line interface for the decompiler. As of right now, Unluau is the most powerful Luau decompiler availible to the public, supporting most of Luau's opcodes and language features. With Unluau you can take a compiled Luau script and restore it to an accurate representation of what the original script may have been.

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
  --dissasemble             (Default: false) Converts the bytecode to a readable
                            list of instructions.

  -o, --output              The file that the decompiled script will be stored
                            in (stdout otherwise).

  -v, --verbose             (Default: false) Shows log messages as the
                            decompiler is decompiling a script.

  --inline-tables           (Default: false) Inlines table definitions. Usually
                            leads to cleaner code.

  --rename-upvalues         (Default: true) Renames upvalues to "upval{x}" to
                            help distinguish from regular local variables.

  --smart-variable-names    (Default: true) Generates logical names for local
                            variables based on their value.

  --help                    Display this help screen.

  --version                 Display version information.

  input file (pos. 0)       Required. Input bytecode file.
  ```
  
  ### Can I contribute?
  Please contribute if you can! My goal was to make this an open source project, so if you have any changes that you would like to implement, feel free to send a pull request and I will review it ASAP.
