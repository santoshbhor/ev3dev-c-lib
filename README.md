# what is this lib for
this lib is to help c# programmers while creating programs for an ev3dev system since the last c# lib became unsupported and unfinnished

# whats needed
ok you need:
  1) Ev3 Brick
  2) a network connection to the Ev3 Birck
  3) a way to PSCP (comes with putty) *SCP can be used but i dont have linux so your on your own*
  4) a way to compile c# code
  5) a Pc (pref running windows)
  6) mono on Ev3 Brick
  
# how to setup mono on Ev3Brick
all you have to do is install mono-complete
  code
  sudo apt-get update -y && sudo apt-get install mono-complete -y
  
  
# what you need to do in short
now what we will be doing to run c# code on the ev3 brick is compile/create the program on a PC and then using PSCP
move the files to the ev3 brick.

### First
create the program and compile it on your pc like any other c# program

### Second
using PSCP move the Ev3Dev.dll, <projname>.exe and anyother files you might need to run <projname>.exe
  Example of using PSCP in cmd
  pscp -pw maker <PathToProj>/<projname>/bin/Ev3Dev.dll robot@<ev3dev ip>: &&
  pscp -pw maker <PathToProj>/<projname>/bin/<projname>.exe robot@<ev3dev ip>:
  
### Third
now you run the code
  In ssh terminal to Ev3 brick
  mono ~/<projname>.exe
and it will work
  
  
if you have any problems please tell me in the issues, i will try to fix as much as posibal.
if you need help getting started please look in the Wiki pages, i will try to keep it as up to date as posibal.
