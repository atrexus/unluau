# Unluau CLI
The Unluau CLI (command line interface) makes it extremely easy to interact with the decompiler without writing any code at all. As of now the CLI is availible for Linux and Windows operating systems. To see if your operating system is supported, check the [releases](https://github.com/valencefun/unluau/tags) tab for a matching binary. 

Once you have located the appropriate binary you can run it by passing it a luau binary file. If you are unfamilliar with luau bytecode generation or simply don't know how to generate it, check out our luau workspace [here](https://github.com/valencefun/luau-workspace).

## Options
The command line interface has a plethora of options availble to customize the behavior of the decompiler. From showing the bare luau bytecode instructions to variable name guessing unluau has it all. Refer to the table of contents below:
* [Input](#input)
* [Output](#output--o---output)
* [Dissasemble](#dissasemble--d---dissasemble)

### Input
A single, optional, argument that determines the source of the bytecode to decompile. To decompile a file your command should look something like this:
```
unluau <inputfile.luau>
```
If you don't end up providing an input file, you will need to provide it via `stdin` (standard input).

### Output `(-o, --output)`
You can direct the output of the decompiler to a file using ``-o`` or `--output`. If this option is not used then the output will just go to stdout (standard out). An example of this option in use can be found below:
```
unluau inputfile.luau -o outputfile.lua
```

### Dissasemble `(-d, --dissasemble)`
When provided, this option will print a dissasembled version of the "assembled" luau machine code to standard out. In simple words it converts the machine code to a somewhat readable format. For example, lets say we have the following script compiled in `Closure.luau`:
```lua
local function Closure()
    print(1)
end

print(Closure)
```
The dissasembled output for the script would be the following:
```
0 param(s), 2 slot(s), 0 upvalue(s), 2 constant(s), 0 function(s)
function Closure() -- line 1 through 2
   GETIMPORT         0 1
      AUX        1073741824
   LOADN             1 1
   CALL              0 2 1
   RETURN            0 1 0
end

0+ param(s), 3 slot(s), 0 upvalue(s), 3 constant(s), 1 function(s)
function main(...) -- line 1 through 5
   PREPVARARGS       0 0 0
   DUPCLOSURE        0 0
   GETIMPORT         1 2
      AUX        1074790400
   MOVE              2 0 0
   CALL              1 2 1
   RETURN            0 1 0
end

Main Function: 1
```
