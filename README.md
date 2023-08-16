# UnluauNET
A decompiler for Roblox's Lua based programming language (Luau https://luau-lang.org/). The Unluau project consists of two different .NET namespaces: ``Unluau`` (the main decompiler) and ``Unluau.CLI`` the command line interface for the decompiler. As of right now, Unluau is the most powerful Luau decompiler availible to the public, supporting most of Luau's opcodes and language features. With Unluau you can take a compiled Luau script and restore it to an accurate representation of what the original script may have been.

Unluau is still in early alpha and is receiving updates frequently. You can download and run any of the apha versions in the [releases](https://github.com/societall/UnluauNET/releases) tab.

## Usage
Currently, you can only use the CLI (command line interface) to decompile bytecode. You can run the following command to decompile any bytecode file of your liking:

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

  --supress-warnings        (Default: false) Does not display warnings to the
                            log file or console.

  --logs                    The file in which the logs for the decompilation
                            will go (uses stdout if not set).

  --inline-tables           (Default: false) Inlines table definitions. Usually
                            leads to cleaner code.

  --rename-upvalues         (Default: true) Renames upvalues to "upval{x}" to
                            help distinguish from regular local variables.

  --smart-variable-names    (Default: true) Generates logical names for local
                            variables based on their value.

  --descriptive-comments    (Default: false) Adds descriptive comments around
                            each block (almost like debug info).

  --help                    Display this help screen.

  --version                 Display version information.

  input file (pos. 0)       Input bytecode file (uses stdin if not provided).
  ```
  
## Benchmarking
To ensure Unluau supports as many Luau language features possible we have created a test suite of various scripts that must pass after every commit to the main branch. These tests can be viewed [here](Unluau.Test/Expect).
