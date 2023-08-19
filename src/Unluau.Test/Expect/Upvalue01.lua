local upval0 = {}
local upval1 = {}
function f()
   local print_v0 = upval0.print
   local print_v1 = upval1.print
   local print_v2 = _ENV.print
   return print
end

function g()
   return _ENV.print
end

function f2(arg1)
   upval0.print = arg1
   upval1.print = arg1
   _ENV.print = arg1
   print = arg1
end

