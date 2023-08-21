local upval0 = {}
upval0.__index = upval0
function upval0.new(arg1)
   local var0 = {}
   var0.name = arg1.name
   var0.items = {}
   print(`Store created with name {var0.name}`)
   return setmetatable(var0, upval0)
end

function upval0.addItem(arg1, arg2)
   local var0 = table.clone(arg2)
   table.insert(arg1.items, var0)
   print(`Item created with name {var0.name}`)
end

local var29 = {}
var29.name = "Shoes"
local var0 = upval0.new(var29)
local var32 = {}
var32.name = "Nike"
var32.stock = 99999
var32.cost = 50
var0:addItem(var32)
local var37 = {}
var37.name = "Gucci"
var37.stock = 10
var37.cost = 500
var0:addItem(var37)
return upval0
