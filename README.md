# Multithreading compressing/decompressing CLI program.

This program made for compressing and decompressing data from different source, with diffrenet algorithms and by using different implementations of processing the data.

I made this program with different components. So every of it can be called without using another one. Except the Computation component, cause it uses the high level classes from compression and IO components.

## Program made of some components:
* IO - Provides high level classes for IO operations.
* Compress - Provides high level classes for compress/decompress.
* Computation - Provides implementation of working with different type of system component for the compression.

## IO component.

This component provides different aproaches to read and write data from sources. it is using the generic fabric method for getting high level class with strategy pattern. So we can specify the concrete implementation of strategy much more later.

### IO Providers:
* Block

### Source Implementation:
* FileBlock

### StrategyInterfaces:
* IBlockProvider

We can specify the needed provider by factory method:

```c#
OFabricMethod<BlockDataProvider>.Create((Enum.DataSource) source, (Enum.DirectionType) direction, string[] payload)
```

## Compression component.

This component provides strategy pattern for compression/decompression implementaions to the algorithms. We can call the high level compression to start implementing our architecure, and then the algorithm will be ready just push it to the high level class by strategy pattern.

### Algorithms:
* GZip

## Computation component.

This component provides different implementations of processing data (how we proccess it). For example i have a disk and cpu usage implementation for the byte block processing.

This component have some fabric methods, some strategy implementation. 

We can implement different algorithms for how to choose the right component in the system for processing and by using the func delagate to implement it.

In this component have an utils class. There you can find and implement or use the methods for system statistics. This data will be used for the algorithms computation.

### Algorithm:
* LowestIdleTime - find the most loaded component by IdleTime statistic.

### Workerks:
* BlockCpuWorker - Use the most loaded core to process the data
* BlockDiskWorker - Use the disk to process the data


## CLI interface.

This program have CLI command processing. Here the info about commands:

* EXIT - Exit from the programm
* INFO - Get info
* Compress - Compress file
* Decompress - Decompress file
* Example - Example of using

To use the programm you need to provide this input parameters:
> Compress -a [R][Algorithm] -provider [R][ProviderType] -i [R][InputType] -ip [R][InputParameters] -o [R][OutputType] -op [R][OutputParameters]

To get more info you need to type something like this:

> INFO Compress -a

Thre result will be:

> Compress/Decompress [Algorithm] - Avaliable compression algorithms:
> Gzip
> Deflate


