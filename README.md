

# Table of Contents
### Introduction
- [What's SkimSkript](#what-is-skimskript)
- [Building the Interpreter](#building-the-interpreter)
- [Your First Program](#your-first-program)
### Syntax Guide
1. [Comments](#1--comments)
2. [Syntax Flexibility](#2--syntax-flexibility)
	- 2.1 [Symbols](#21--symbols)
 	- 2.2 [Float Period/Decimal Rules](#22--float-perioddecimal-rules)
	- 2.3 [Whitespace and Lines](#23--whitespace-and-lines)
 	- 2.4 [Keywords/Phrases](#24--keywordsphrases)
3. [Data Types](md#3--data-types)
	- 3.1 [Integers](#31--integers)
 	- 3.2 [Floating-Points](#32--floating-points)
  	- 3.3 [Booleans](#33--booleans)
   	- 3.4 [Strings](#34--strings)
4. [Variables]
	- 4.1 [Identifiers](#41--identifiers)
 	- 4.2 [Variable Declarations](#42--variable-declarations)
  	- 4.3 [Variable Assignments]
   	- 4.4 [Variable Scope]
5. [Control Structures]
	- 5.1 [If Statements]
 	- 5.2 [If-Else Statements]
  	- 5.3 [Else-If Statements]
  
# Introduction

## What is SkimSkript?
SkimSkript is an interpreted language designed for beginners. It features flexible, forgiving syntax and enforced typing with runtime coercion, allowing users to write in C-style braces, Python-like keywords, or even natural-language pseudocode. SkimSkript adapts to the programmer — not the other way around.

With support for multiple syntactic styles, it serves as a stepping stone to more advanced languages. Beginners can experiment with static types, recursion, and procedural logic without being boxed into rigid syntax rules or overwhelmed by compiler errors.

### Programmer Freedom
Would you like to stick to something more reminiscent of the **C family of languages**? You can!
```csharp
int i = 1;

while(i <= 100)
{
    if(i % 15 == 0)
	print("FizzBuzz");
    else if(i % 3 == 0)
	print("Fizz");
    else if(i % 5 == 0)
	print("Buzz");
    else
	print(i);
	
 i = i + 1;
}
```

Maybe you want to mix in a little **Pythonic** syntax:
```python
def int factorial(int n) =>
    if n == 0:
	return 1
    else:
	return n * factorial(n - 1)

print(factorial(5))  # Output: 120
```

You may want something more **verbose, academic, or closer to written English**. In that case, SkimSkript has got you covered:   
```
Declare int iterator as 1.
 
Run print("Please enter a number to find its square root:").
Declare integer input as value of read().

Invoke print:("Square root" plus value of SquareRoot(input))

Define integer function SquareRoot(integer number):
{
	Declare integer odd as 1. Declare integer count as 0.
	
	Repeat while number is at least 0:
	{
		 Set number to number minus odd.
		 Set odd to odd plus 2. 
		 Set count to count plus 1.
	}

	Give back count minus 1.
}
```

### How Does It Work?
The SkimSkript interpreter utilizes no third-party libraries, so while it possesses the typical lexical analysis, AST parsing, and interpretation that many other tree interpreters have, it also has some quirks that enable its unique syntactic style.

A prime example of this occurs during lexical analysis. After scanning each character in the source code, any alphabetic lexemes are fed into a trie structure. If a single alphabetic lexeme is a partial match for a phrase, the next lexeme is analyzed to check if it is part of that phrase as well. If a group of lexemes reaches a trie node containing a token type, those lexemes are grouped as a single token (assuming the following word does not continue a phrase). However, if the lexemes that form a partial match do not complete a phrase, then each of those lexemes is marked as an identifier. This unconventional approach allows for multi-word tokens, simplifies parsing, and accommodates multiple syntactic styles.

## Building the Interpreter
1. Assuming you have the **.NET 8 SDK** and have downloaded the **SkimSkript repository**, set the current working directory to the repository folder that contains ```SkimSkript.csproj``` using the terminal of your choosing. Your terminal should look something like this:
	##### Windows PowerShell:
	```powershell
	PS C:\User\Sebastian> cd SkimSkript
	```
   
	##### Bash:
	```bash
	sebastian@computer:~$ cd SkimSkript
	```
2. Once in the same directory as the C# project file, type ```dotnet build``` and enter.
	##### Windows PowerShell:
	```powershell
	PS C:\User\Sebastian\SkimSkript> dotnet build
	```
	
	##### Bash:
	```bash
	sebastian@computer:~/SkimSkript$ dotnet build
	```

3. If the interpreter compiled correctly, you should see a message in your terminal that resembles the following:
	```powershell
	  Determining projects to restore...
	  All projects are up-to-date for restore.
	  SkimSkript -> 	 C:\User\Sebastian\SkimSkript\bin\Debug\net8.0\skimskript.dll
	
	Build succeeded.
	    0 Warning(s)
	    0 Error(s)
	
	Time Elapsed 00:00:00.74
	```
	**Note**: If any yellow warning text appears in the terminal after executing the command, there is no need to fear. So long as you can see ```Build succeeded``` (possibly among the warning(s)) and ```Time Elapsed XX:XX:XX.XX``` at the bottom of the message; then, the interpreter should be compiled and ready to go. 
<br><br>
**Congrats!** You can now use the SkimSkript interpreter! We'll discuss the two different approaches in the next section, where we create your first SkimSkript program.


## Your First Program
1. First create a **.skim** file. For the sake of this tutorial we're calling the file ```HelloWorld.skim```. This file will contain your SkimSkript code.
   
2. Open ```HelloWorld.skim``` in your IDE of choice.
 
3. Now with the file open, you can write a classic "Hello World" program by writing ```print``` followed by an open parenthesis, then ```"Hello World"``` (include the double quotes), and finish with a closing parenthesis:	
	```python
	print("Hello World")
	```
 
4. Save your modified ```HelloWorld.skim```
 
5. Now let's run ```HelloWorld.skim``` in the interpreter.

	Set your current working directory to the repository folder that contains ```SkimSkript.csproj```. Then, use the ```dotnet run``` command and pass the ```HelloWorld.skim path``` as an argument.

	##### Windows PowerShell:
	```powershell
	PS C:\User\Sebastian\SkimSkript> dotnet run -- HelloWorld.skim
	```
	
	##### Bash:
	```bash
	sebastian@computer:~/SkimSkript$ dotnet run -- HelloWorld.skim
	```
	**(If HelloWorld.skim is located elsewhere, use the full or relative path to the file instead.)**
   
7. If you see what's below then congratuations! You coded your first SkimSkript program!
	```
	Hello World

	C:\Users\Sebastian\Desktop\SkimSkript\bin\Debug\net8.0\skimskript.exe (process 27136) exited with code 0 (0x0).
	To automatically close the console when debugging stops, enable Tools->Options->Debugging->Automatically close
	the console when debugging stops.
	Press any key to close this window . . .
	```
## 1. ) Comments
 - To comment, use the pound sign (i.e., ```#```) at the end of a line followed by your comment:

	```python
	# This is a comment
	```

## 2. ) Syntax Flexibility	
- ### 2.1 ) Symbols
	When it comes to non-alphabetic and non-numeric symbols, SkimSkript gives you quite a bit of leeway. You can use any symbol not on this table without interfering with your program.

	#### Reserved Symbols
	|+   | -  |*   | /  |%   | >  |
	| ------------ | ------------ | ------------ | ------------ | ------------ | ------------ |
	|  < |=   | !=  |==   |&#124;&#124;   |&&   |
	| ^^  | "|
	
- ### 2.2 ) Float Period/Decimal Rules
	**Periods can affect your program if not used correctly.** Periods/decimals are used in floating-point numbers, but in most cases are ignored. 
	- Single float --> ```1.0``` --> Expected Syntax
	- Single float --> ```1.0...``` --> Extra periods are ignored
	- Single integer --> ```.1``` --> Floats start with digits
	- Two integers --> ```1..0``` --> Second periods are interpreted as trailing an int

- ### 2.3 ) Whitespace and Lines
	As long as there is at least a single space between alphabetic and numeric lexemes, the interpreter ignores spacing and break lines:
	```csharp
	    int      myVar =      20
	           myVar =        20
	    if(myVar 
	 ==        50) {print("My Variable is 50")}
	```

- ### 2.4 ) Keywords/Phrases
	SkimSkript keywords have some quirks:
	- **Only complete phrases and singular keywords are reserved.** Meaning using a single word from a reserved phrase is allowed as an identifier unless the whole phrase is present. For example, ```give back``` is reserved, but you can use ```give``` and ```back``` as variable names.
	- **Reserved words and phrases are NOT case-sensitive**. So, ```Give back``` , ```iF```, and ```INTEGER``` are all valid.

	#### Reserved Keywords
	
	|  run |  invoke |  return | true  | false  | if  |
	| ------------ | ------------ | ------------ | ------------ | ------------ | ------------ |
	|  else |  elif |  otherwise | while  | def  |  ref |
	| declare  | set  | to  | as  | int  | integer  |
	|  float | bool  | boolean  |  string | is  |  or | 
	|  and | then  | minus  |  plus |   
## 3. ) Data Types

- ### 3.1 ) Integers
	Whole numbers that truncate digits to the right of the decimal point. The data type can be written as ```int``` or ```integer```. Literals can be purely digits or a negative sign followed by digits. Examples: ```29```, ```5```, and ```-300```.

- ### 3.2 ) Floating-Points
	Numbers with digits to the right of a single decimal point. The data type can be written as ```float``` or ```floating point```. The literals are an optional negative sign followed by one or more digits, a single decimal, and trailing digit(s). Examples: ```1.0```, ```432.992```, and ```-32.12```.

- ### 3.3 ) Booleans
	True or false values. The data type can be written as ```bool``` or ```boolean```. The literals are either ```true``` or ```false```.

- ### 3.4 ) Strings
	Stored text that can be appended to, printed, or read in through user input. The data type can be written as ```string``` The literals are characters enclosed in double parenthesis. Example: ```"This is a string literal!"```

## 4. ) Variables
SkimSkript contains scoped variables declared in top-level, function, or control structure blocks.

- ### 4.1 ) Identifiers
	Identifiers are tokens at least one character long that start with an alphabetic character. Following the first character can be digits or alphabetic characters until whitespace. **Identifiers ARE case sensitive.**

	- ```a``` -->Valid identifier
	- ```myVar2``` --> Valid identifier
	- ```2Var``` --> Invalid identifier
	- ```My Var``` --> Two seperate identifiers

- ### 4.2 ) Variable Declarations
	You can declare variables by identifying a type and, optionally, an initial value.

	 - #### Verbose
		```csharp
		declare integer myVariable as 10
		```
	- #### Brief
		```csharp
		float myVariable2 = 10
		```
	- #### Mix
		```bash
		int myVariable3 as 10
		declare int myVariable4 = 10
		integer myVariable5 = 50
		declare string myVariable6
		```

- ### 4.3 ) Variable Assignments
	Variables can be initialized during declaration or reassigned after. 
	
	**Note:** Values assigned to a variable will be coerced to be the variable type. If the coercion is invalid, an exception will be thrown.

	- #### Verbose
		```python
		declare string myString1 as "Uses 'as' to initialize variable during declaration"
		set myString2 to "Uses 'set', identifier, and 'to'"
		```
	- #### Brief
		```csharp
		bool myBool = false
		myBool = true
		```
	- #### Mix
		```csharp
		 float myFloat
		 set myFloat = 20.0
		    
		 int myInt as 5
		 myInt to myFloat
		```
  
- ### 4.4 ) Variable Scopes
 	- #### Global Scope
		Variables declared on the top level are considered global. So you can access the variable from anywhere (including functions.))

		```python
		string globalVar = "I can be accessed anywhere after my declaration!"
		ModifyGlobalVar()
		print(globalVar) # Output: I was modified by a function!!!
		
		def ModifyGlobalVar()
		{
			globalVar = "I was modified by a function!!!"
		}
		```
 
	- #### Local Scope
		Variables declared in a function body are only accessible within that body (the function's block and nested blocks). Similarly, parameters of a function are only accessible within that function's body but, conversely, are declared in the function definition.

		```python
		# Parameter "rightOperand" can only be used inside the block below
		def AddFiveAndPrint(int rightOperand) 
		{
			 #Variable "sum" can be used anywhere in this function after declaration
			 int sum = 5 + rightOperand
			    
			 if sum < 0
			 print("Sum was negative! Sum: " + sum)
			 else if sum == 0
			 print("Sum was not negative or positive! Sum: " + sum)
			 else
			 print("Sum was positive! Sum: " + sum)
			 print("x" + sum)
		}
		
		# Neither the parameter or variable above can be used here
		AddFiveAndPrint(20)
		```

	- #### Block Scope
		Inside control structure blocks, any variables declared are only to be accessed from that block or a nested block.

		```python
		print("Please enter a positive number to multiply by -1:")
		int userNumber = read()
		
		if userNumber >= 1
		{
			 # You can use "negativeProduct" in this block beyond the line below
			 int negativeProduct = userNumber * -1
			    
			 print("Product of your number and -1: " + negativeProduct)
			    
			 print("Want to see the product multiplied by -1 again? (y for yes):")
			
			 if read() == "y"
			 # negativeProduct can be accessed here as well
			 print("Product of the previous product and -1: " + negativeProduct * -1)
		}
		else
		 print("Error: Number entered was not positive")
		    
		```
 
## 5. ) Control Structures
- ### 5.1 ) Conditional Statements
	- #### If Statements 
		If statements work virtually the same as any other language, but you have the option to use the ```then``` keyword.
		
		```csharp
		if myCondition is true then
			print("myCondition is true")
			
		if myCondition2 is false
			print("myCondition2 is false")
		```
	
	- #### If-Else Statements
		If-else statements give you a choice of two different keywords to serve as "else." ```else``` and ```otherwise``` that can be used interchangably.
		
		```python
		# This can either be the verbose or brief "if." The interpreter will accept either.
		if myCondition is true 
			print("myCondition is true")
		else
			print("MyCondition is false")
			
		if myCondition2 is false
			print("myCondition2 is false")
		otherwise
			print("MyCondition2 is true")
		```
 
	- #### Else-If Statements
		Else-if statements have many variants to choose from. You can use ```else if```, ```elif```, ```instead if```, ```alternatively if```, and ```otherwise if```.
		
		```python
		print("Please pick a number from 0-5:")
		int userSelection = read()
		
		if userSelection is 0
			print("You selected 0")
		else if userSelection is 1 then # You can optionally use then at the end of any variant
			print("You selected 1")
		elif userSelection is 2
			print("You selected 2")
		instead if userSelection is 3
			print("You selected 3")
		alternatively if userSelection is 4
			print("You selected 4")
		otherwise if userSelection is 5
			print("You selected 5")
		else
			print("Selection outside of valid range")
		```
 
 - ### 5.4 ) While Loops
	While loops provide three different syntactic options to choose from before your loop condition. ```while```, ```repeat while```, and ```repeat code while```.

	```python
	int iterator1 = 0
	
	while iterator1 < 5
	{
		iterator1 = iterator1 + 1
		print("iterator1: " + iterator1)
	}
	
	int iterator2 = 0
		
	repeat while iterator2 < 5
	{
		iterator2 = iterator2 + 1
		print("iterator2: " + iterator2)
	}
	
	int iterator3 = 0
		
	repeat code while iterator3 < 5
	{
		iterator3 = iterator3 + 1
		print("iterator3: " + iterator3)
	}
	```
 
- ### 5.5 ) Control Structure Blocks
	Control structures have two separate "types" or "styles" of blocks you can choose depending on your preference.

	- #### Explicit Blocks
		You can define an explicit block by an opening and closing curly brace (i.e., ```{``` and ```}```.) Explicit blocks can contain one or more statements. However, a block must be explicit if it includes more than one statement (as mentioned in the implicit blocks section).

		```python
		bool isRunning = true
		
		while isRunning is true
		{ 
			#Block with multiple statements
			print("Hello!")
			print("Want me to greet you again? y for yes")
			
			if read() is not "y"
			{ 
				#Block with a single statement
				isRunning = false
			}
		}
		```
	- #### Implicit Blocks
		If not defined by opening and closing curly braces, then the interpreter will treat an expected block as implicit. Unlike explicit blocks, only singular statements are allowed by the interpreter in implicit blocks.

		```python
		bool isRunning = true
		int iterator = 0
		
		
		print("What value do you want the iterator to reach from 1 to 1000?")
		int maxValue = read()
		
		if maxValue <= 1000 or maxValue >= 1
		 # Control structures can be a statement in implicit blocks as well
			while isRunning is true
			{ # A control structure inside an implicit block can itself use implicit or explicit blocks
				print("Iterator is " + iterator)
				if iterator is 5000:
					isRunning = false
				else
					iterator = iterator + 1
			}		
		```
  
## 6. ) Functions
SkimSkript offers statically typed support for both user-defined and built-in functions.

- ### 6.1 ) Function Definitions
	- #### Brief
		A brief function definition begins with the keyword ```def```, followed by an optional ``` data type```, a case-sensitive ```identifier```, parentheses ```()``` (even if empty), and a ```block```.
	
		```python
		def NoReturnTypeFunction() { }
		
		def int IntFunction() { return 1 }
		
		def float FloatFunction() { return 1.0 }
		
		def string StringFunction() { return "Hello" }
		
		def bool BoolFunction() { return true }
		```

	- #### Verbose
		A verbose function definition starts with the keywords ```define function``` followed by an optional data-type, ```identifier```, parenthesis ```()``` (even if empty), and a block. 
		
		```javascript
		define function NoReturnTypeFunction()
		
		define integer function IntFunction() { give back 1 }
		
		define floating point function FloatFunction() { give back 1.0 }
		
		define string function StringFunction() { give back "Hello" }
		
		define boolean function BoolFunction() { give back true }
		```
	 - #### Mix
		And of course, you can combine the syntaxes.

		```python
		def integer GetMeaningOfLife() { give back 42 }
		
		define PrintHelloWorld() { print("Hello World") }
		
		define bool IsThisAFunction() { return true }
		
		def floating point function GetFloatValue() { give back 30.0 }
		```

- ### 6.2 ) Return Statements
	- #### Void Functions
		A function can exit at any point in its body using a return statement — either the keyword ```return``` or the phrase ```give back```. For void functions (functions that don't return a value), return statements are optional but can still be used for clarity or early exits.

		```python
		def ExitEarly()
		{
			# Immediately exits scope regardless of any remaining statements
			return
		
			# This statement can't be reached
			print("Nothing will be printed.") 
		}
		
		def ExitInNestedBlock()
		{
			if true
			{
				print("I'm going to return from inside a control structure!")
		
				# Even inside a nested block a return statement will exit
				# the function entirely when executed
				return
			}
		
			# Control normally could exit an if statement block after executing it,
			# but, because of the return, control exits the function inside the if block
			print("I can't be printed")
		}
		```

	- #### Value-Returning Function
		If a function is defined to return a value, it must use return or give back followed by an ```expression``` before exiting. That expression must match the declared return type or be coercible to it at runtime. If it doesn't, the interpreter will raise an error.

		```python
		def int ReturnOnePlusOne()
		{
			# Returns an integer value of 2
			return 1 + 1
		}
		
		def float ReturnMyVariable()
		{
			float myVar = 32.0
		
			# Returns value of variable which is the float 32.0
			return myVar
		}
		
		def float ReturnStringAsFloat()
		{
			# Coerces the string to a float value of 132.12
			return "132.12"
		}
		
		def int FailToReturnValue()
		{
			# Will throw an exception because the string does not match the pattern of an int
			return "This string literal cannot be coerced into an int"
		}
		```

- ### 6.3 ) Function Parameters
	Functions accept both statically typed pass-by-value and pass-by-reference parameters.

	- #### Pass-by-Value Parameters
		Parameter declarations appear inside the parentheses to the left of the identifier. A ```data type``` followed by an ```identifier``` denotes a pass-by-value parameter. To declare multiple parameters, repeat this pattern to the right of the first.

		```python
		def PrintString(int stringParameter) {
			print("String argument: " + stringParameter)
		}
		
		def int GetSum(int left int right) {
			return left + right
		}
		```
	- #### Pass-by-Reference Parameters
		Pass-by-reference parameters follow the same rules as pass-by-value, with one key difference: pass-by-reference declarations require the use of ```ref``` or ```reference to``` before the data type.

		```python
		def IncrementInts(ref int int1 ref int int2) {
			int1 = int1 + 1
			int2 = int2 + 1
		}
		
		def IncrementFloats(reference to float float1 reference to float float2) {
			float1 = float1 + 1
			float2 = float2 + 1
		}
		```
- ### 6.3 ) Function Calls + Arguments
	To call a function, use its identifier followed by opening and closing parenthesis. If it has parameters, list the expressions or identifiers you'd like to send as arguments inside the parenthesis. If a parameter is passed by reference, the argument forwarded **must be an identifier** and must be labeled using ```ref``` or ```reference to```.

	- #### Brief
		```csharp
		PrintString("This is a string argument!")
		
		int myInt1 = 1
		int myInt2 = 2
		
		int sum = GetSum(myInt1 myInt2) + 5
		
		IncrementInts(ref myInt1 ref myInt2)
		```	
	 - #### Verbose
 		 Optionally when calling a function outside an expression you can use ```run``` before the identifier. Inside an expression you optionally use ```value from```.
    
		```csharp
		run PrintString("This is a string argument!")
		
		int myInt1 = 1
		int myInt2 = 2
		
		int sum = value from GetSum(myInt1 myInt2) + 5
		
		run IncrementInts(reference to myInt1 reference to myInt2)
		```
	**Note:** Function calls can appear **before or after** their definitions. Functions can also be **recursive**, calling themselves within their own body.
