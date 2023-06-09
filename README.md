# Tiktoken

[![Nuget package](https://img.shields.io/nuget/vpre/Tiktoken)](https://www.nuget.org/packages/Tiktoken/)
[![dotnet](https://github.com/tryAGI/Tiktoken/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/tryAGI/Tiktoken/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/github/license/tryAGI/Tiktoken)](https://github.com/tryAGI/Tiktoken/blob/main/LICENSE.txt)
[![Discord](https://img.shields.io/discord/1115206893015662663?label=Discord&logo=discord&logoColor=white&color=d82679)](https://discord.gg/Ca2xhfBf3v)

### Implemented encodings
- `cl100k_base`
- `r50k_base`
- `p50k_base`
- `p50k_edit`

### Usage
```csharp
var encoding = Tiktoken.Encoding.ForModel("gpt-4");
var tokens = encoding.Encode("hello world"); // [15339, 1917]
var text = encoding.Decode(tokens); // hello world
var numberOfTokens = encoding.CountTokens(text); // 2

var encoding = Tiktoken.Encoding.Get("p50k_base");
var tokens = encoding.Encode("hello world"); // [31373, 995]
var text = encoding.Decode(tokens); // hello world
```

## Benchmarks
You can view the reports for each version [here](benchmarks)

<!--BENCHMARKS_START-->
``` ini

BenchmarkDotNet=v0.13.5, OS=macOS Ventura 13.3.1 (a) (22E772610a) [Darwin 22.4.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK=7.0.203
  [Host]     : .NET 7.0.5 (7.0.523.17405), Arm64 RyuJIT AdvSIMD
  Job-XASHAI : .NET 7.0.5 (7.0.523.17405), Arm64 RyuJIT AdvSIMD DEBUG

BuildConfiguration=Debug  

```
|              Method |                Data |         Mean |      Error |     StdDev | Ratio |     Gen0 |     Gen1 |   Gen2 |  Allocated | Alloc Ratio |
|-------------------- |-------------------- |-------------:|-----------:|-----------:|------:|---------:|---------:|-------:|-----------:|------------:|
|   **SharpTokenV1_0_28** | **1. (...)57. [19866]** | **5,265.610 μs** | **91.4786 μs** | **81.0934 μs** |  **1.00** | **601.5625** | **296.8750** |      **-** | **3716.57 KB** |        **1.00** |
| TiktokenSharpV1_0_5 | 1. (...)57. [19866] | 1,661.705 μs |  7.4719 μs |  5.8336 μs |  0.32 | 253.9063 | 128.9063 | 3.9063 | 1534.33 KB |        0.41 |
|            Tiktoken | 1. (...)57. [19866] | 1,358.612 μs |  6.9032 μs |  6.1195 μs |  0.26 | 253.9063 | 128.9063 | 3.9063 | 1534.33 KB |        0.41 |
|                     |                     |              |            |            |       |          |          |        |            |             |
|   **SharpTokenV1_0_28** |       **Hello, World!** |     **3.278 μs** |  **0.0102 μs** |  **0.0091 μs** |  **1.00** |   **0.6752** |   **0.0038** |      **-** |    **4.14 KB** |        **1.00** |
| TiktokenSharpV1_0_5 |       Hello, World! |     6.646 μs |  0.0152 μs |  0.0127 μs |  2.03 |   2.1820 |   0.0458 |      - |   13.41 KB |        3.24 |
|            Tiktoken |       Hello, World! |     6.334 μs |  0.0199 μs |  0.0166 μs |  1.93 |   2.1820 |   0.0458 |      - |   13.41 KB |        3.24 |
|                     |                     |              |            |            |       |          |          |        |            |             |
|   **SharpTokenV1_0_28** | **King(...)edy. [275]** |    **66.560 μs** |  **0.1570 μs** |  **0.1311 μs** |  **1.00** |   **8.5449** |   **0.4883** |      **-** |   **52.89 KB** |        **1.00** |
| TiktokenSharpV1_0_5 | King(...)edy. [275] |    21.811 μs |  0.0651 μs |  0.0577 μs |  0.33 |   5.0964 |   0.3052 |      - |   31.34 KB |        0.59 |
|            Tiktoken | King(...)edy. [275] |    18.263 μs |  0.0818 μs |  0.0683 μs |  0.27 |   5.0964 |   0.3052 |      - |   31.34 KB |        0.59 |

<!--BENCHMARKS_END-->
