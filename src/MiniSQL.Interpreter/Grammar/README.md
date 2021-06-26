# ANTLR definition

Generate lexer and parser.

## Prerequisite

First, install ANTLR sdk.

Then make some alias command to the PATH.

```cmd
REM antlr.cmd
@echo off
echo.
java -jar "PATH\TO\antlr-4.8-complete.jar" %*
```

```cmd
REM antlrc.cmd
javac -classpath .;"PATH\TO\antlr-4.8-complete.jar" *.java
```

```cmd
REM grun.cmd
java -cp .;"PATH\TO\antlr-4.8-complete.jar" org.antlr.v4.gui.TestRig %*
```

```powershell
dotnet add package Antlr4.Runtime.Standard
```

## Generate code

Set `pwd` to current folder first. Then run the following command.

```
antlr -Dlanguage=CSharp -visitor -o MiniSQL MiniSQL.g4
```

## Make customized visitor

1. create new class file
1. inherit it from IYOURGRAMMARNAMEVisiter<object>
1. resolve errors
1. inherit it from YOURGRAMMARNAMEBaseVisiter<object>
1. add keyword `override` to all methods
1. delete unnecessary methods
1. implement remaining methods

## Visualize

Set `pwd` to current folder and run the following command.

```cmd
antlr -o MiniSQL_Java MiniSQL.g4
cd MiniSQL_Java
antlrc
grun MiniSQL prog -tree -gui ..\tests\table-select-1.sql
cd ..
rm -r MiniSQL_Java

```
