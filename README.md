# Redstone Logic
[![Redstone Logic](About/Preview.png)](https://steamcommunity.com/sharedfiles/filedetails/?id=2991569144)

Not official Minecraft product. Not approved by or associated with Mojang or Microsoft.

## redstone dust

![](screens/redstone_ore.png)

a mysterious glowing mineral from a parallel universe. an essential component of all redstone-operated mechanisms.

obtaining:
- small lumps of redstone ore will be generated on new maps
- ground-penetrating scanner will find lumps of buried redstone on existing maps
- trade caravans
- quest rewards
- redstone mining worksites (ideology)

## redstone wire

![](screens/redstone_wire.png)

similar to electric conduit, redstone wire transmits redstone power.
power level drops by 1 for every cell of redstone wire.
thus, redstone wire can transmit power for no more than 15 cells. 

## generators

![](screens/generators.png)

- **redstone torch** - generates signal of level 15, cannot be moved, never burns out
- **redstone block** - generates signal of level 15, can me moved, can be pushed by pistons
- **lever** - generates signal of level 15 when switched on, cannot be moved

## daylight detector

![](screens/detectors.gif)

produces redstone power in proportion to the daylight cycle. can be switched to moonlight mode. is not affected by roofs or artificial light sources.


## tripwire hook

![](screens/tripwire.png)

emits a redstone signal when any pawn or item appears between two hooks.
one hook can have up to 4 links.
floor-level buildings, like pressure plates, will not trigger the hook.


## pressure plates

![](screens/pressure_plates.png)

wooden pressure plates can detect all entities.

stone pressure plates can detect only pawns/animals.

golden ("light") detect all entities, and the signal strength equals the number of entities stood on one.

steel ("heavy") is similar to golden, but measures groups of 10 entities.


## tnt

![](screens/tnt.png)

an explosive block with logic pretty similar to minecraft's one.
can be ignited by redstone signal, fire or explosion.
best combined with tripwires or pressure plates :)

## repeater

![](screens/repeaters.gif)

repeats incoming signal with a configurable 1..250 ticks delay.


## block

![](screens/blocks.png)

just a simple stuffable block that can be pushed by pistons. think of it as a movable wall.


## piston

![](screens/pistons.png)

**piston**:
- pushes any items into an empty cell
- pushes acceptable items into a storage
- breaks any plants/trees, producing harvested resources, if any
- pushes pawns
- if pawns are pushed against the wall, they receive blunt damage
- pushes blocks
- pushes other pistons if they're not extended

**sticky piston**:
- pushes all things similar to a regular one
- pulls blocks / pistons back

by default pistons will not push or pull most of vanilla **buildings**. i've added example support to vanilla torch lamp, campfire and glow pod, which will be just breaked by a piston.

## extending

if building is simple and does not have any caching logic in **postspawn()**, then this should be sufficient:

      <modextensions>
        <li class="redstonelogic.extpistonmoveable"/>
      </modextensions>

if you want building to break when pushed by a piston:

      <modextensions>
        <li class="redstonelogic.extpistonmoveable">
          <breaks>true</breaks>
        </li>
      </modextensions>

if building has some internal logic/caching, and just changing it's position is not sufficient: (true for all storage buildings)

      <modextensions>
        <li class="redstonelogic.extpistonmoveable">
          <respawn>true</respawn>
        </li>
      </modextensions>

## verified compatible/supported mods

- [lwm's deep storage](https://steamcommunity.com/sharedfiles/filedetails/?id=1617282896)
- [blocky signs](https://steamcommunity.com/sharedfiles/filedetails/?id=2985030059)

## you may also like...

[![loft bed](https://steamuserimages-a.akamaihd.net/ugc/2030602392616950419/caf6f6ab4c5d99e729ad70c683c0d78169b028bf/?imw=268&imh=151&ima=fit&impolicy=letterbox)](https://steamcommunity.com/sharedfiles/filedetails/?id=2961708299)
[![yada](https://steamuserimages-a.akamaihd.net/ugc/2031731300519719867/4e551b5e8a5f51182bd2d8830c7e9e180d0634bc/?imw=268&imh=151&ima=fit&impolicy=letterbox)](https://steamcommunity.com/sharedfiles/filedetails/?id=2971543841)
[![gene collector qol](https://steamuserimages-a.akamaihd.net/ugc/2031731627304502175/d4cbb7ce5a2acd29fe85b5993b7ce209b944389f/?imw=268&imh=151&ima=fit&impolicy=letterbox)](https://steamcommunity.com/sharedfiles/filedetails/?id=2978672610)

https://github.com/zed-0xff/rw-redstonelogic

## support me

[![ko-fi](https://i.imgur.com/utx6oih.png)](https://ko-fi.com/k3k81z3w5) or [patreon](https://www.patreon.com/zed_0xff)
