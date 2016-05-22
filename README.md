LessIO
========
This is a library for dealing with File I/O on .NET that overcomes some limitations of .NET's System.IO libraries (such as long path names) and aspires to be platform independent and require less time to ramp up on than System.IO.

It was created to factor out some of the file-system specific features that have built up in the [LessMSI project](https://github.com/activescott/lessmsi).

Goals
========
* Support File I/O operations on Windows that .NET's System.IO libraries fail to support such as long paths (those longer than 260 characters).
* Provide a basis for platform independent file system access across both Windows and Unix-like systems such as Linux and Mac OSX, and potentially others (cloud file storage?).

Concepts & Usage
========
Two concepts are necessary to use the library `FileSystem` and `Path`. The static `FileSystem` class is a static class that contains all of the operations available on the `FileSystem`. Any operation that has a path argument, such as `FileSystem.CreateDirectory`, accepts paths as strongly typed `Path` objects rather than strings. For example, 

    Path destDir = new Path(@"c:\src\lessmsi");
    FileSystem.CreateDirectory(destDir);

Having paths strongly typed rather than strings forces the caller to be more explicit and I believe leads to less errors in the code. `Path` also normalizes weird paths such as those with double directory seperators so that paths like c:\src\\lessmsi and c:\src\\lessmsi are equal when compared:

    new Path(@"c:\src\\lessmsi") == new Path(@"c:\src\lessmsi") // true

`Path` has some handy shortcuts on it too such as Remove (delete) that calls back into the FileSystem to remove the file or directory at the current path:

    Path destDir = new Path(@"c:\src\lessmsi");
    destDir.Remove();

`Path` also offers some of the commonly used static methods on System.IO.Path to make the type relatively compatible with existing code using System.IO.Path. For example, `Combine` and `GetFileName` are available to make porting System.IO code easier:

    Path.GetFileName(@"c:\src\lessmsi\README.md"); // README.md
    Path.Combine(@"c:\src\lessmsi", "README.md"); // c:\src\lessmsi\README.md


Platform Independence
========
The current implementation supports Win32 via the [Win32FileSystemStrategy.cs](https://github.com/activescott/LessIO/blob/master/src/LessIO/Strategies/Win32/Win32FileSystemStrategy.cs). However, adding support for another file system such as a Unixy file system could be done by implementing the ~10 methods on a class derived from [FileSystemStrategy](https://github.com/activescott/LessIO/blob/master/src/LessIO/Strategies/FileSystemStrategy.cs) and then updating the implementation of [FileSystem.LazyStrategy](https://github.com/activescott/LessIO/blob/master/src/LessIO/FileSystem.cs).
