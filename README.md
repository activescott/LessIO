LessIO
========
This is a library for dealing with File I/O on .NET that overcomes some limitations of .NET's System.IO libraries (such as long path names) and aspires to be platform independent and require less time to ramp up on than System.IO.

Goals
========
* Support File I/O operations on Win32 that .NET's System.IO libraries do not such as long file names (those longer than 260 characters).
* Provide a basis for platform indipendent file system access across both windows and unix-like systems such as Linux and Mac OSX. 

