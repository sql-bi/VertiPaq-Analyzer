# Dax.Vpax.CLI

This is a .NET tool that provides CLI access to [VertiPaq-Analyzer](https://github.com/sql-bi/VertiPaq-Analyzer) functions.

Operations supported by this tool are:

- Export a VPAX file from a tabular model.

## How to install the tool

The tool can be installed using the following command:

````bash
dotnet tool install Dax.Vpax.CLI --global
````

## How to run the tool

````bash
vpax export "C:\output\file.vpax" "Provider=MSOLAP;Data Source=<SERVER>;Initial Catalog=<DATABASE>"
````

Use `vpax -?` or `vpax export -?` to learn more.

```
Description:
  Export a VPAX file from a tabular model

Usage:
  vpax export <path> <connection-string> [options]

Arguments:
  <path>               Path to write the VPAX file
  <connection-string>  Connection string to the tabular model

Options:
  --overwrite     Overwrite the VPAX file if it already exists [default: False]
  --exclude-bim   Exclude the BIM model (Model.bim) from the export [default: False]
  --exclude-vpa   Exclude the VPA model (DaxVpaView.json) from the export [default: False]
  -?, -h, --help  Show help and usage information
```
