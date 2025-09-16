# SkimSkript Programming Language Interpreter
## What is SkimSkript?
SkimSkript is an experimental interpreted language meant to explore different syntactical possibilities. It features flexible and forgiving syntax, allowing users to write in C-style braces, Python-like keywords, or even code that is reminiscent of written English. SkimSkript adapts to the programmer.

SkimSkript's interpreter is entirely written from scratch without the assistance of any third-party libraries or tools, allowing for fine-tuned and unique features not found in other interpreters. With support for multiple syntactic styles, programmers can experiment with reference parameters, recursion, and procedural logic without being boxed into rigid syntax rules or overwhelmed by interpreter/compiler errors.

#### ***[Click here to read the Wiki to see in-depth details.](https://github.com/SebastianBathrick/SkimSkript/wiki)***

## Flexible Syntax
The language supports syntax similar to C-family languages:
```csharp
int Factorial(int number)
{
	if (number == 0 || number == 1)
	{
		return 1;
	}
	else
	{
		return number * Factorial(number - 1);
    }
}
```

You can achieve a Python-like style by utilizing alternate keywords & ignored characters:
```python
def int factorial(int number):
	if number == 0 or number == 1:
		return 1
	else:
		return number * factorial(number - 1)
```

Code can take the form of pseudo code composed of written English by using [phrases](https://github.com/SebastianBathrick/SkimSkript/wiki/1%E2%80%902:-Comments-&-Syntax-Intricacies#24--keywordsphrases) & [ignored characters](https://github.com/SebastianBathrick/SkimSkript/wiki/1%E2%80%902:-Comments-&-Syntax-Intricacies#21--symbols)
```
Define integer function f(Integer 'n') :
If n is 0 or n is 1, return 1. Otherwise, return n multiplied by the value 
of f(n-1).
```

Syntactic conventions, like the ones shown above, can be combined to make custom programming styles:
```python
declare bool isRunningUser as true

while isRunningUser
	try {
		int selectedNum = read("Please enter a whole number:")
		int resultNum = Fibonacci(selectedNum)

		# Print the result.
		print("Fibonacci number for " + selectedNum + " is " + resultNum)

		# Ask the user if they want to enter a new number.
		Set isRunningUser to the value of EnterNewNumber()
	}
	raise print("Invalid input. please try again.") 

Define int function Fibonacci(int number):
	If the number is at most 1, then return the number. Otherwise, return the value of 
	Fibonacci(number-1) plus Fibonacci(number-2).

def boolean function EnterNewNumber()
	give back read("Would you like to enter a new number? (y/n):") is "y"
```
## Features
|   |   |   |   |   |
|:---:|:---:|:---:|:---:|:---:|
| Comments | Alt. Syntaxes | Flexible Whitespace | Optional Symbols | Static Typing |
| Integers | Floating-Points | Booleans | Strings | Runtime Coercion |
| String Concatenation | Variable Declarations | Variable Initialization | Variable Assignments | Top-level Scope |
| Local Scope | Block Scope | If Statements | Else-If Statements | Else Statements |
| While Loops | Single Statement Blocks | Void Functions | Value-Returning Functions | Value + Reference Parameters |
| I/O Built-in Functions | Recursion | Assertions | Try-Catch Statements | Exceptions |
| Comparison Operators | Logical Operators | Arithmetic Operators | Operator Precedence | Nested Expressions |
### Features Coming Soon
- Arrays
- Casting
- File I/O
- Structs
- User-Defined Exceptions
- More Flexible Identifiers

## Quick Start
### Requirements
* **.NET 8.0 SDK**
* **Git** (for build info generation)
* No external dependencies
Quick Start
bash

### Interpreter Setup
```bash
git clone https://github.com/SebastianBathrick/SkimSkript.git
cd SkimSkript
dotnet restore
dotnet build
```

### Your First Program

## Basic Architectural Overview
### [SkimSkriptCore Class](https://github.com/SebastianBathrick/SkimSkript/blob/main/SkimSkriptCore.cs)
Instantiates, caches, and exchanges data between the Lexer, Parser, and Interpreter to form a MainComponent pipeline. In essence, it's the orchestrator for each step from source code to execution.

### [MainComponent](https://github.com/SebastianBathrick/SkimSkript/blob/main/MainComponents/MainComponent.cs)
Base class for all primary components that perform the main functionality of the SkimSkript interpreter (Lexer, Parser, and Interpreter classes). This class features functionality to aid in the development and debugging of the SkimSkript interpreter, including features like displaying MainComponent inputs, outputs, and execution times in the terminal. Use <u>**--help**</u> as a command-line flag/argument for more information about MainComponent debugging.

### [Lexer Class](https://github.com/SebastianBathrick/SkimSkript/blob/main/MainComponents/Lexer.cs)
Performs lexical analysis to convert source code to [Tokens](https://github.com/SebastianBathrick/SkimSkript/blob/main/Tokens/Token.cs) and caches them in a [TokenContainer](https://github.com/SebastianBathrick/SkimSkript/blob/main/Tokens/TokenContainer.cs) that will be sent to the Parser.

### [Parser Class](https://github.com/SebastianBathrick/SkimSkript/blob/main/MainComponents/Parser.cs)
Utilizes tokens to build an abstract syntax tree (AST) composed of [Nodes](https://github.com/SebastianBathrick/SkimSkript/blob/main/Nodes/Node.cs) for interpretation. 

### [Interpreter Class](https://github.com/SebastianBathrick/SkimSkript/blob/main/MainComponents/Interpreter.cs)
Handles the execution of the program by recursively traversing the AST and utilizing its data.

### MainComponent Pipeline Diagram
The following is a diagram of the general architecture of the entire program. 
<img width="1520" height="380" alt="InterpreterDesign (6)" src="https://github.com/user-attachments/assets/fa7b5a69-53de-417a-8867-2a8c6d82acc1" />

#### [***You can read more about the interpreter's architecture here.***](https://github.com/SebastianBathrick/SkimSkript/wiki/*-Architectural-Overview)
