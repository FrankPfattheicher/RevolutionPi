# Variable Server
The Variable Server is a simple REST-API http-server providing access to the Revolution Pi variables.

## Requirements
* Mono 5.x installed [see "Installing Mono"](InstallMono.md)

## Usage
Create VariableServer.exe from sources using VisualStudio (Community Edition) on a PC
or load the prebuild executables as ZIP file.

[Prebuild executable: VariableServer.zip](VariableServer.zip)

Copy project output or zip contents to a folder on the RevolutionPi.

Start the server using the following commandline

    mono VariableServer.exe

## API
Open Browser and navigate to http://localhost:8000/    
The server should answer with the default document

**GET /**
```json
{
  "service": "RevolutionPi Variable Server",
  "varlist": "GET ~/variables",
  "readvar": "GET ~/variables/{varname}"
}
```

To query the server for a list of configured variables use the route /variables.    
For each variable its name, type and length is returned.

**GET /variables**
```json
[
  {
    "name": "RevPiStatus",
    "type": "Input",
    "length": "BYTE"
  },
    ...
  {
    "name": "RS485ErrorLimit2",
    "type": "Input",
    "length": "WORD"
  }
]
```

To query the value of a specific variable use the route /variables/{varname}.    
The variable's name, type, length and value is returned.

**GET /variables/RevPiStatus**
```json
{
  "name": "RevPiStatus",
  "type": "Input",
  "length": "BYTE",
  "value": 1
}
```
If the variable name is unknown, the response code is NotFound (404).

**GET /variables/Unknown**
```json
{
  "error": "Variable not found",
  "name": "Unknown"
}
```
If there is a read error, the response code is ServiceUnavailable (503).

**GET /variables/ReadError**
```json
{
  "error": "Could not read variable",
  "name": "ReadError"
}
```
