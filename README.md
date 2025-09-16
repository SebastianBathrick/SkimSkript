# SkimSkript Programming Language Interpreter
## What is SkimSkript?
SkimSkript is an experimental interpreted language meant to explore different syntactical possibilities. It features flexible, forgiving syntax and enforced typing with runtime coercion, allowing users to write in C-style braces, Python-like keywords, or even code that is reminiscent of written English. <u>***SkimSkript adapts to the programmer — not the other way around.***</u>

SkimSkript's <u>interpreter is entirely written from scratch</u> without the assistance of any third-party libraries, allowing for unique features not found in other interpreters. With support for multiple syntactic styles, programmers can experiment with reference parameters, recursion, and procedural logic without being boxed into rigid syntax rules or overwhelmed by interpreter/compiler errors.

#### ***[Click here to read the Wiki to see in-depth details.](https://github.com/SebastianBathrick/SkimSkript/wiki)***

## Programmer Freedom
***Would you like to stick to something more reminiscent of the <u>***C family of languages***</u>? You can!***
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

***Maybe you want to mix in a little <u>***Pythonic***</u> syntax:***
```python
def int factorial(int number):
	if number == 0 or number == 1:
		return 1
	else:
		return number * factorial(number - 1)
```

***You may want something more <u>***verbose***</u>, like something similar to a <u>***proof***</u>, or closer to <u>***written English***</u>. In that case, SkimSkript has got you covered:***   
```
Define integer function f(Integer 'n') :
If n is 0 or n is 1, return 1. Otherwise, return n multiplied by the value 
of f(n-1).
```

***And even better, you can <u>***mix & match syntax***</u> to <u>***tailor your own style***</u>, making your <u>***code truly your own***</u>:***
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
|   |   |   |   |   |   |   |
|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| Comments | Alt. Syntaxes | Flexible Whitespace | Optional Symbols | Static Typing | Integers | Floating-Points |
| Booleans | Strings | Runtime Coercion | String Concatenation | Variable Declarations | Variable Initialization | Variable Assignments |
| Top-level Scope | Local Scope | Block Scope | If Statements | Else-If Statements | Else Statements | While Loops |
| Single Statement Blocks | Void Functions | Value-Returning Functions | Value + Reference Parameters | I/O Built-in Functions | Recursion | Assertions |
| Try-Catch Statements | Exceptions | Comparison Operators | Logical Operators | Arithmetic Operators | Operator Precedence | Nested Expressions |
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

```bash
git clone https://github.com/SebastianBathrick/SkimSkript.git
cd SkimSkript
dotnet restore
dotnet build
dotnet run
```
