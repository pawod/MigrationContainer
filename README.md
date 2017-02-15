# Table of Contents
1. [About MigrationContainer](#1-about-migrationcontainer)

2. [Features](#2-features)

3. [Background](#3-background)

4. [Use of Migration Containers](#4-use-of-migration-containers)

   - [Container Types](#4-1-container-types)
   - [Extending Container Types](#4-2-extending-container-types)
   - [Exporting Data (Creating Containers)](#4.3-exporting-data-creating-containers)
   - [Importing Data (Extracting Data From Containers)](#4.4-importing-data-extracting-data-from-containers)

5. [Technical Details](technical-details)
   - [Container Format](#container-format)
   - [Single-Part Containers](#single-part-containers)
   - [Multi-Part Containers](#multi-part-containers)
<br>

# 1. About MigrationContainer
MigrationContainer is a C# serialization framework based on [Marc Gravell's Implementation](https://github.com/mgravell/protobuf-net) of Google Protocol Buffers.  It aims to create container files imaging file systems and meta data from various sources for one of following purposes:



- Data Migration
- Creating system snapshots, e.g. backups of user accounts



Still not clear about the purpose of MigrationContainer? Imagine following scenario:

You run a cloud storage solution and plan to change your system's architecture, e.g. by switching from an NTFS based file system to an Amazon S3 hosted storage. This would require the migration of existing user accounts to the new environment. More precisely this would require:



- Backup of all account related information residing in various systems, e.g. Active Directory, database
- Backup of all files related meta data such as *Alternate Data Streams*, *Junction Points*, creation date, etc.
- Backup of the folder hierarchy in a user's storage
- Import of all mentioned information/data to the new environment



## 2. Features

- Multi or single-part container files with configurable file size

* Mapping of entire directory hierarchies
* Powered by [Google Protocol Buffers](https://developers.google.com/protocol-buffers/)
  * fast & powerful serializer 
  * extensible container format
  * backwards-compatible container format
* Support for NTFS Alternate Data Streams & Junction Points



## 3. Background

This fr`amework is the result of a request made by [Teamplace](https://www.teamplace.net/en/). Simultaneously it also served as topic for my bachelor's [thesis](https://drive.google.com/open?id=0B_-vg-Ca4cDReUQ5LTZvWk0zemc). With kind permission of Teamplace the main framework is now released to the public. 

Probably MigrationContainer could be converted into a Mono or DNX project to be used on further platforms, not only Windows as currently.

More details on MigrationContainer's background, its use and the underlying container format can be found in the wiki or the [thesis](https://drive.google.com/open?id=0B_-vg-Ca4cDReUQ5LTZvWk0zemc) itself.



## 4. Use of Migration Containers

With MigrationContainer you can create and access physical *migration containers*. Each of these consists of two logical units: 

1. The **header** section describing the container’s contents and providing important meta-information cruicial to the migration procedure.
2. The **body** containing the actual data to be migrated.

If you are planning to access or even create containers outside this project, you will need to make yourself familiar with the  container's specifications. See the second part of this wiki: [Container Format Specification](#2-container-format-specification) for more details.

### 4.1 Container Types
MigrationContainer provides two basic types of containers:

1. `NtfsFileContainer` : Creates an image of a single NTFS file and its meta data.
2. `NtfsDirectoryContainer`: Creates an image of an entire NTFS directory including all files and subdirectories.


### 4.2 Extending Container Types

**Defining a New Container Type**

All types of containers must inherit from the abstract class` MigrationContainer<TChild, THeader, TBody>`, which provides the basic functionality for (de-)serialization.

`TChild`: Used to implement the [Curiously recurring template pattern](https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern). In other words, this should be the type of the container you want to implement.

`THeader`: A type inheriting from `IFileHeader`, which specifies the basic functionality header describing a file's meta data. Note that `DirectoryHeaders` are a special type of `FileHeaders`.

`TBody`: A type inheriting from `IContainerBody`. It provides the functionality for access and (de-)serialization of content stored in a container's body. For more details on a migration container's format see the [container format specification]().

Also note that each container class should be decorated with the `ContainerMetaDescription` attribute to specify the format's file extension.

**Registering a Container Type to the Protobuf Type Hierarchy**

For proper (de-)serialization of class hierarchies protobuf requires to assign a unique number to each type called *tag*. For that puprose the `ProtoConfiguration` class provides the `LargestTagInUse` property to keep track of this number.  To register your new container type to the existing container hierarchy, you need to use one of following methods:

- `AddSubType(Type basetype, Type derivedType, int protoTag)`
- `AddSubTypeForGenericBaseType(Type genericBaseType, Type baseBaseType, Type derivedType, int protoTag)`

Use `LargestTagInUse + 1` as `protoTag` when registering a new type. The property is updated automatically, on successful registration. For more information read the method's respective XML-doc.

It is important that you perform this registration for all of your types at startup of your program, **BEFORE** you any of your types is attempted to be initialized (caution: static variables). Here is an example how you could accomplish this:

    /// <summary>
    ///     Helper class for establishing a consistent hierarchy of ProtoHeaders that are provided by this project.
    /// </summary>
    public class MyProtoInheritance
    {
        private static MyProtoInheritance _instance;
        private static MyProtoInheritance Instance => _instance ?? (_instance = new MyProtoInheritance());
    
        private MyProtoInheritance()
        {
            AddSubTypes();
        }
    
        /// <summary>
        ///     Initializes the protobuf inheritance hierarchy for all ProtoHeaders defined by this project if called for the first
        ///     time. Subsequent calls have no effect.
        /// </summary>
        public static void Initialize()
        {
            // adds all subtypes to inheritance hierarchy
            var foo = Instance;
        }
    
        private void AddSubTypes()
        {
            // content headers
            ProtoConfiguration.Instance.AddSubType(typeof(AlternateStreamHeader),
                                                   typeof(FileVersionHeader),
                                                   ProtoConfiguration.Instance.LargestTagInUse + 1);
            ProtoConfiguration.Instance.AddSubType(typeof(FileHeader), typeof(NtfsStorageFileHeader), ProtoConfiguration.Instance.LargestTagInUse + 1);
    
            ProtoConfiguration.Instance.AddSubTypeForGenericBaseType(
                typeof(DirectoryHeader<NtfsStorageDirectoryHeader, NtfsStorageFileHeader, NtfsStorageFile, NtfsStorageDirectory>),
                typeof(FileHeader),
                typeof(NtfsStorageDirectoryHeader),
                ProtoConfiguration.Instance.LargestTagInUse + 1);


            // other headers
            ProtoConfiguration.Instance.AddSubType(typeof(ProtoHeader), typeof(StorageHeader), ProtoConfiguration.Instance.LargestTagInUse + 1);
            ProtoConfiguration.Instance.AddSubType(typeof(ProtoHeader), typeof(FileCommentHeader), ProtoConfiguration.Instance.LargestTagInUse + 1);
        }
    }
    
### 4.3 Exporting Data (Creating Containers)

**TODO**

### 4.4 Importing Data (Extracting Data From Containers)

**TODO**
