## What is SkimSkript?
SkimSkript is an interpreted language designed for beginners. It features flexible, forgiving syntax and enforced typing with runtime coercion, allowing users to write in C-style braces, Python-like keywords, or even natural-language pseudocode. SkimSkript adapts to the programmer — not the other way around.

With support for multiple syntactic styles, it serves as a stepping stone to more advanced languages. Beginners can experiment with static types, recursion, and procedural logic without being boxed into rigid syntax rules or overwhelmed by compiler errors.

### Programmer Freedom
Would you like to stick to something more reminiscent of the C family of languages? You can!
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

Maybe you want to mix in a little Python syntax:
```python
def int factorial(int n) =>
    if n == 0:
        return 1
    else:
        return n * factorial(n - 1)

print(factorial(5))  # Output: 120
```

You may want something more verbose, academic, or closer to written English. In that case, SkimSkript has got you covered:   
```
Declare int iterator as 1.
 
Run print("Please enter a number to find its square root:").
Declare integer input as value of read().

Invoke print:("Square root" plus value of SquareRoot(input))

Define integer function SquareRoot(integer number):
{
 Declare integer odd as 1. Declare integer count as 0.

 Repeat while number is at least 0
 {
 Set number to number minus odd.
 Set odd to odd plus 2. 
 Set count to count plus 1.
 }

 Give back count minus 1.
}
```

### How Does the Interpreter Do All This?
The SkimSkript interpreter utilizes no third-party libraries, so while it possesses the typical lexical analysis, AST parsing, and interpretation that many other tree interpreters have, it also has some quirks that enable its unique syntactic style.

A prime example of this occurs during lexical analysis. After scanning each character in the source code, any alphabetic lexemes are fed into a trie structure. If a single alphabetic lexeme is a partial match for a phrase, the next lexeme is analyzed to check if it is part of that phrase as well. If a group of lexemes reaches a trie node containing a token type, those lexemes are grouped as a single token (assuming the following word does not continue a phrase). However, if the lexemes that form a partial match do not complete a phrase, then each of those lexemes is marked as an identifier. This unconventional approach allows for multi-word tokens, simplifies parsing, and accommodates multiple syntactic styles.

### Is the Codebase Overcomplicated?

Not at all. SkimSkript was built with maintainability and clarity in mind. The bulk of the interpreter was developed between October and December 2024 with a strong focus on clean architecture and minimal dependencies.

As of May 2025:
- ~800 lines of executable code
- Maintainability Index: 88
- No third-party libraries used

### What Features Does it Have?
As of now, SkimSkript has the following:
- Statically typed variable declarations & assignments.
- User-defined functions with optional statically typed return types.
- Global, local, and block scopes.
- Reference and value parameters.
- Recursive functions
- Nested expressions.
- String concatenation.
- Runtime coercion.
- IO built-in functions
- Exponents
- Semantic analysis
- Ignored Symbols
- Implicit blocks


### What Features are Coming Next?
The following features I'd like to add next are:
<u>**(More Detailed +Beginner Guides Coming Soon)**</u>
- More in-depth + novice guides
- More advanced semantic analysis
- Increment operator
- For loops
- Do while loops
- Switch control structure
- Composite types
- File IO built-in functions
- String escape characters

# Guide 

## Your First Program
You can write a classic "Hello World" program by writing ```print``` followed by an open parenthesis, then "Hello World" in double quotes, and finish with a closing parenthesis:
```python
print("Hello World")
```
Then in a terminal of your choice type the path of **skimskript.exe** and your **.skim file path** as an argument. It should look something like this:
```bash
PS C:\Users\sebas> skimskript HelloWorld.skim
```
## Ignored Content
### Comments
To comment, use the pound sign (i.e. ```#```) at the end of a line followed by your comment:
```python
# This is a comment
```

### Symbols
When it comes to non-alphabetic and non-numeric symbols, SkimSkript gives you quite a bit of leeway. Essentially, you can use any symbol not on this table without interfering with your program. <br><br>
**Reserved Symbols:**
|+   | -  |*   | /  |%   | >  |
| ------------ | ------------ | ------------ | ------------ | ------------ | ------------ |
|  < |=   | !=  |==   |&#124;&#124;   |&&   |
| ^^  | "|

**Note on Periods**: <u>Periods can affect your program if not used correctly.</u> Periods are used in floating-point numbers. 

**Examples:**

- Single float --> ```1.0``` --> Expected Syntax
- Single float --> ```1.0...``` --> Extra periods are ignored
- Single integer --> ```.1``` --> Floats start with digits
- Two integers --> ```1..0``` --> Second periods are interpreted as trailing an int

### Whitespace and Lines
As long as there is at least a single space between alphabetic and numeric lexemes, the interpreter ignores spacing and break lines:
```csharp
    int      myVar =      20
           myVar =        20
    if(myVar 
 ==        50) {print("My Variable is 50")}
```

### Keywords/Phrases
SkimSkript keywords have some quirks:
- **Only complete phrases and singular keywords are reserved.** Meaning using a single word from a reserved phrase is allowed as an identifier, unless the whole phrase is present. For example, ```give back``` is reserved, but you can use ```give``` and ```back``` as variable names.
- **Reserved words and phrases are NOT case-sensitive**. So, ```Give back``` , ```iF```, and ```INTEGER``` are all valid.

**Reserved Keywords:**

|   |   |   |   |   |   |
| ------------ | ------------ | ------------ | ------------ | ------------ | ------------ |
| run  | invoke  | return  | true  | false  | if  |
|  else |  elif |  otherwise | while  | def  |  ref |
| declare  | set  | to  | as  | int  | integer  |
|  float | bool  | boolean  |  string | is  |  or | 
|  and | then  | minus  |  plus |   
## Data Types
Right now SkimSkript offers 4 different data types to choose from:
### Integers
Whole numbers that truncate digits to the right of the decimal point. The data type can be written as ```int``` or ```integer```. Literals can be purely digits or a negative sign followed by digits. Examples: ```29```, ```5```, and ```-300```.

### Floating-Points
Numbers with digits to the right of a single decimal point. The data type can be written as ```float``` or ```floating point```. The literals are an optional negative sign followed by one or more digits, a single decimal, and trailing digit(s). Examples: ```1.0```, ```432.992```, and ```-32.12```.
### Booleans
True or false values. The data type can be written as ```bool``` or ```boolean```. The literals are either ```true``` or ```false```.
### Strings
Stored text that can be appended to, printed, or read in through user input. The data type can be written as ```string``` The literals are characters enclosed in double parenthesis. Example: ```"This is a string literal!"```
## Variables
SkimSkript contains scoped variables declared in top-level, function, or control structure blocks.
### Identifiers
Identifiers are tokens at least one character long that start with an alphabetic character. Following the first character can be digits or alphabetic characters until whitespace. **Identifiers ARE case sensitive.** 

**Examples:**
- ```a``` -->Valid identifier
- ```myVar2``` --> Valid identifier
- ```2Var``` --> Invalid identifier
- ```My Var``` --> Two seperate identifiers

### Variable Declarations
Variables are declared by identifying a type and optionally an initial value.
#### Verbose
```bash
declare integer myVariable as 10
```
#### Brief
```csharp
float myVariable2 = 10
```
#### Mix
```bash
int myVariable3 as 10
declare int myVariable4 = 10
integer myVariable5 = 50
declare string myVariable6
```

### Variable Assignments
Variables can be initialized during declaration or reassigned after. **Note:** Values assigned to a variable will be coerced to be the variable type. If the coercion is invalid, an exception will be thrown.
####Verbose
```python
declare string myString1 as "Uses 'as' to initialize variable during declaration"
set myString2 to "Uses 'set', identifier, and 'to'"
```
#### Brief
```csharp
bool myBool = false
myBool = true
```
#### Mix
```
 float myFloat
 set myFloat = 20.0
    
 int myInt as 5
 myInt to myFloat
```
### Variable Scopes
SkimSkript contains three separate scope types that determine where you can use certain variables and parameters.
#### Global Scope
Variables declared on the top level are considered global. Meaning you can access the variable from anywhere (including functions.)
```csharp
string globalVar = "I can be accessed anywhere after my declaration!"
```
#### Local Scope
Variables and parameters declared as part of a function definition/body can only be used inside that function.
```
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

#### Block Scope
Variables declared inside of a control structure can be used within that control structure (including nested control structures)
```
print("Please enter a positive number to multiply by -1:")
int userNumber = input()

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
## Control Structures
### If Statements
If statements work virtually the same as any other language, but you have the option to use the ```then``` keyword.
```csharp
if myCondition is true then
	print("myCondition is true")
	
if myCondition2 is false
	print("myCondition2 is false")
```

### If-Else Statements
If-else statement's give you a choice of two different keywords to serve as "else". ```else``` and ```otherwise``` that can be used interchangably.
```
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
### Else-If Statements
Else if statements have many variants to choose from for the sake of variety. You can use ```else if```, ```elif```, ```instead if```, ```alternatively if```, and ```otherwise if```.
```
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
### While Loops
While loops provide three different options syntactic options to choose from before your loop condition. ```while```, ```repeat while```, and ```repeat code while```.
```
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
### Control Structure Blocks
Control structures have two seperate types of blocks you can choose from.
#### Explicit Blocks
Explicit blocks are defined by an opening and closing curly brace (i.e. ```{``` and ```}```.) Explicit blocks can contain one or more statements. However, a block must be explicit if it contains more than one statement.
```csharp
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
#### Implicit Blocks
If not using curly braces only one statement will be considered in that block.
```
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
## Functions
SkimSkript offers both statically typed user defined functions and built-in functions.

**Note: Definitions without a return type are void.**
### Function Definitions
A **brief** function definition starts with the keyword ```def``` followed by a data-type (or lack thereof), a case sensitive```identifier```, ```opening + closing parenthesis```, and a block. 

```
def NoReturnTypeFunction() { }

def int IntFunction() { return 1 }

def float FloatFunction() { return 1.0 }

def string StringFunction() { return "Hello" }

def bool BoolFunction() { return true }
```
A **verbose** function definition starts with the keywords ```define function``` followed by a data-type (or lack thereof), ```identifier```, ```opening + closing```, and a block. 

```
define function NoReturnTypeFunction()

define integer function IntFunction() { give back 1 }

define floating point function FloatFunction() { give back 1.0 }

define string function StringFunction() { give back "Hello" }

define boolean function BoolFunction() { give back true }
```
And of course, you can combine the syntaxes.
```
def integer GetMeaningOfLife() { give back 42 }

define PrintHelloWorld() { print("Hello World") }

define bool IsThisAFunction() { return true }

def floating point function GetFloatValue() { give back 30.0 }
```

### Function Parameters
Functions accept both statically typed pass-by-value and pass-by-reference parameters
