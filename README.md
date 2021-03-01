# cardano-json-metadata
A .NET utility library and command-line application (CLI) for converting JSON to and from Cardano transaction metadata

### JSON Schemas
We've designed a JSON schema to validate Cardano transaction metadata files in another repository: https://github.com/fullcircl/schemas/tree/main/cardano/draft/tx-metadata.json

## CardanoJsonMetadata
CardanoJsonMetadata is a .NET Standard 2.0 portable class library for converting non-metadata JSON data into Cardano metadata schema data.

## CardanoJsonMetadata.Cli
CardanoJsonMetadata.Cli is a command-line executable program for validating and converting JSON data to and from Cardano meta-data schema.

## Status
cardano-json-metadata is currently in early development stages. The schemas, class library, and CLI are all in early alpha stage, and are not suitable for use on a mainnet.

## Pre-requisites
[.NET 5 SDK]

## Development environment

We recommend Visual Studio 2019 for easy debugging.

Clone this repo, git clone https://github.com/fullcircl/cardano-json-metadata.git
