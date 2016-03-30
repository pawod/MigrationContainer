# Data Migrator

The Data Migrator is a framework, which enables you to create platform independent container files for the purpose of data migration or backups. It is implemented in C# (.NET Framework 4.5) and uses [Marc Gravell's Implementation](https://github.com/mgravell/protobuf-net) of Google's Protocol Buffers for (de-)serialization of aforementioned containers.


###What it does
This framework helps you to perform automated data migrations between heterogeneous environments, which require incompatible data(-structures) to be mapped in some way to achieve a semantically lossless transfer of data.

* It helps you to gather distributed data and puts it into a single container. You could also use such containers as images for backup purposes. Provided you have a suitable mapping schema these backups are insensitive to future architectural changes of your system.
* It helps you to transfer data across environmental boundaries.
* It helps you to import that data into the targeted system by using a suitable mapping schema defined by yourself


### Why do I need this?
Imagine you are running some kind of cloud service or complex system which has grown historically over many years. It stores tons of persistent data spread around various components using different technologies.
As your service grows you will probably hit the limits of your system's architecture at some point of time. You might be forced to make significant changes in order to preserve your service's performance and maintainability. However this will also require to transfer all existing data to the new environment. With increasing technical divergence this will become a complex task as there is no trivial one-to-one mapping between source and target system. This is where the Data Migrator comes in handy.


### Features
* multi or single-part container files with configurable file size
* mapping of entire directory hierarchies
* Powered by [Google Protocol Buffers](https://developers.google.com/protocol-buffers/)
	* fast & powerful serializer 
	* easily extensible container format
	* backwards-compatible container format
* supports NTFS Alternate Data Streams & Junction Points


### Note
This framework is the result of a request made by [Teamplace](https://www.teamplace.net/en/). Simultaneously it also served as topic for my bachelor's [thesis](https://drive.google.com/open?id=0B_-vg-Ca4cDReUQ5LTZvWk0zemc). With kind permission of Teamplace the main framework is now released to the public. 

Probably the DataMigrator could be converted into a Mono project to be used on further platforms, not only Windows as currently.

More details on the Data Migrator's background, its use and the underlying container format can be found in the wiki or the [thesis](https://drive.google.com/open?id=0B_-vg-Ca4cDReUQ5LTZvWk0zemc) itself.