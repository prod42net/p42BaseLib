# p42BaseLib

p42BaseLib is a small .NET 9.0 library (C# 13) providing lightweight base utilities used across projects:
- A simple, extensible logger (P42Logger + IP42Logger)
- A fixed-size queue implementation (FixedSizedQueue)
- Interfaces for object model storage and logging (IP42ObjectModelStore, IP42Logger)

These components are intentionally minimal and dependency-free so they can be dropped into other projects easily.

## Key Features
- P42Logger: lightweight logging abstraction suitable for console apps, libraries, or to be wired into larger logging systems.
- FixedSizedQueue: a bounded queue that automatically evicts oldest items when capacity is exceeded.
- Clear interfaces to help decouple implementations and make unit testing straightforward.

## Requirements
- .NET 9.0 SDK
- C# 13.0 compatible toolchain

## Building
From the repository root:
1. Restore dependencies:
   dotnet restore

2. Build the solution:
   dotnet build

## Usage
- Add a project reference to p42BaseLib (or include the library project in your solution).
- Use IP42Logger to accept different logger implementations or use P42Logger directly for simple scenarios.
- Use FixedSizedQueue<T> when you need a thread-unsafe bounded queue for short-lived buffering (wrap for thread-safety if required).

Example (conceptual):