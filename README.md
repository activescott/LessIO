LessIO
========
[![Build status](https://ci.appveyor.com/api/projects/status/0qp51hmyyurv22ss?svg=true)](https://ci.appveyor.com/project/activescott/lessio)
[![nuget](https://img.shields.io/nuget/v/LessIO.svg)](https://www.nuget.org/packages/LessIO/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/lessio.svg?style=popout)](https://www.nuget.org/packages/LessIO/) 



This is a library for dealing with File I/O on .NET that overcomes some limitations of .NET's System.IO libraries (such as long path names) and aspires to be platform independent and require less time to ramp up on than System.IO.

It was created to factor out some of the file-system specific features that have built up in the [LessMSI project](https://github.com/activescott/lessmsi).


Goals
========
* Support File I/O operations on Windows that .NET's System.IO libraries fail to support such as long paths (those longer than 260 characters).
* Provide a basis for platform independent file system access across both Windows and Unix-like systems such as Linux and Mac OSX, and potentially others (cloud file storage?).

Install
========
Install [via NuGet](https://www.nuget.org/packages/LessIO/).

Concepts & Usage
========
Two concepts are necessary to use the library `FileSystem` and `Path`. The static `FileSystem` class is a static class that contains all of the operations available on the `FileSystem`. Any operation that has a path argument, such as `FileSystem.CreateDirectory`, accepts paths as strongly typed `Path` objects rather than strings. For example:

    Path destDir = new Path(@"c:\src\lessmsi");
    FileSystem.CreateDirectory(destDir);

Having paths strongly typed rather than strings forces the caller to be more explicit and I believe leads to less errors in the code. `Path` also normalizes weird paths such as those with double directory seperators so that paths like `c:\src\\lessmsi` and `c:\src\\lessmsi` are equal when compared:

    new Path(@"c:\src\\lessmsi") == new Path(@"c:\src\lessmsi") // true

`Path` has some handy shortcuts on it too such as Exists that calls back into the FileSystem to determine if the file or directory at the current path already exists. 
These methods also provides some compatibility with existing code using System.IO.FileInfo or System.IO.DirectoryInfo so that in many cases you can replace usage of System.IO.FileInfo/DirectoryInfo with LessIO.Path.

    Path destDir = new Path(@"c:\src\lessmsi");
    Console.WriteLine(destDir.Exists);

`Path` also offers some of the commonly used static methods on System.IO.Path to make the type relatively compatible with existing code using System.IO.Path. For example, `Combine` and `GetFileName` are available to make porting System.IO code easier:

    Path.GetFileName(@"c:\src\lessmsi\README.md"); // README.md
    Path.Combine(@"c:\src\lessmsi", "README.md"); // c:\src\lessmsi\README.md


Platform Independence
========
The current implementation supports Win32 via the [Win32FileSystemStrategy.cs](https://github.com/activescott/LessIO/blob/master/src/LessIO/Strategies/Win32/Win32FileSystemStrategy.cs). However, adding support for another file system such as a Unixy file system could be done by implementing the ~10 methods on a class derived from [FileSystemStrategy](https://github.com/activescott/LessIO/blob/master/src/LessIO/Strategies/FileSystemStrategy.cs) and then updating the implementation of [FileSystem.LazyStrategy](https://github.com/activescott/LessIO/blob/master/src/LessIO/FileSystem.cs).


Contributing
========
We accept pull requests! I think this is a sound basis, but obviously there are many improvements that could be made such as [improving platform independence by adding a new FileSystemStrategy](#platform-independence) or adding more static methods to [Path](https://github.com/activescott/LessIO/blob/master/src/LessIO/Path.cs) from System.IO.FileInfo/DirectoryInfo to make it easier to port System.IO code over to this library. There might also be some important methods missing on [FileSystem](https://github.com/activescott/LessIO/blob/master/src/LessIO/FileSystem.cs) as I just added the operations that [LessMSI](https://github.com/activescott/LessMSI) already uses which I imagine is fairly extensive but maybe not comprehensive.

Please do make sure that existing tests pass and please add new ones for the new features you write.


Deploying
========
Deployment to NuGet and GitHub Releases is automated. To deploy a new release take the following steps:

1. Make sure all changes are merged to master
2. Checkout the commit from the master branch. Then tag the commit with the tag `publish` by running the following command: `git tag -f publish`
3. Push the tag with `git push --tags -f`

The build script will then detect the tag, build it and publish it to GitHub Releases and Nuget.
