local upval3 = {}
local upval4 = {}
function f()
   local print_v0 = upval3.print
   local print_v1 = upval4.print
   local print_v2 = _ENV.print
   return print
end

function g()
   return _ENV.print
end

function f2(arg1)
   upval3.print = arg1
   upval4.print = arg1
   _ENV.print = arg1
   print = arg1
end

