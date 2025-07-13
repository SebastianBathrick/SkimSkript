# SkimSkript Language Documentation

> **Note:** This project was originally named "BardBytes," but has been renamed to SkimSkript. Some files might still reference BardBytes.

## Table of Contents
1. [Introduction](#1-introduction)
2. [Lexical Structure](#2-lexical-structure)
3. [Data Types](#3-data-types)
4. [Variables](#4-variables)
5. [Expressions](#5-expressions)
6. [Control Structures](#6-control-structures)
7. [Functions](#7-functions)
8. [Standard Library Functions](#8-standard-library-functions)
9. [Scope](#9-scope)
10. [Type Coercion](#10-type-coercion)
11. [Compilation Process](#11-compilation-process)
12. [Examples](#12-examples)
13. [Error Handling](#13-error-handling)

## 1. Introduction

SkimSkript is a statically-typed, interpreted programming language designed to be accessible for beginners while incorporating modern programming concepts. It features a flexible syntax that allows both explicit and shorthand styles, making it easier to transition to other programming languages like C#, Java, Python, or JavaScript.

### Key Features
- Statically typed with automatic type coercion
- Both explicit and shorthand syntax options
- Natural language-inspired keywords
- Object-oriented architecture (interpreter implementation)
- Built-in error handling with descriptive messages
- Forgiving syntax with many characters being ignored

### Architecture Overview

SkimSkript follows a traditional compiler/interpreter architecture with the following phases:
1. **Lexical Analysis**: Processes source code into tokens (using `Lexer`, `Scanner`, and `Evaluator`)
2. **Syntax Analysis**: Builds an Abstract Syntax Tree (AST) with the `Parser` class
3. **Semantic Analysis**: Validates code semantics through the `SemanticAnalyzer`
4. **Interpretation**: Executes the code using the `Interpreter` class

## 2. Lexical Structure

### A. Ignored Characters

One unique feature of SkimSkript is that the interpreter treats most characters other than alphabetic, operators, or numeric as whitespace (aside from string literals or comments). This flexibility gives programmers freedom to format their code in a way that makes sense to them or helps with transitioning to other languages.

For example, you can add semicolons or commas (which are ignored) to practice C-family syntax:

```
int count = 5;
while (count > 0) {
    print(count);
    count = count - 1;
}
```

Or use Python-like syntax with colons:

```
int count = 5
while count > 0:
    print(count)
    count = count - 1
```

### B. Comments

Comments in SkimSkript are inline and start with the `#` symbol. Everything following the `#` on the same line is ignored by the interpreter.

```
# This is a single-line comment
print("Hello World") # Inline comment after code

# Multi-line comment example:
# Each line must start with a `#`
# to continue the comment.
```

### C. Identifier Rules

Identifiers in SkimSkript follow these rules:

1. **Case-Sensitivity**: Identifiers are case-sensitive. `myVar`, `MyVar`, and `MYVAR` are treated as different identifiers.
2. **First Character**: Must start with an alphabetic character (a-z, A-Z).
3. **Subsequent Characters**: Can include alphabetic characters, digits (0-9), and underscores.
4. **Valid Examples**: `count`, `firstName`, `total_sum`, `value2`
5. **Invalid Examples**: `2count` (starts with a digit), `my-var` (contains a dash)

Example:
```
int Count = 5    # Different from 'count'
int count = 10   # Different from 'Count'
```

### D. Reserved Keywords

SkimSkript has several reserved keywords that cannot be used as identifiers:

```
def, int, float, string, bool, true, false, if, else, else if, instead if,
otherwise, while, repeat, return, give back, run, value of, declare, set, 
to, as, is, is not, is at least, exceeds, is at most, is below, or, and, 
or just, then, ref, reference to
```

### E. Spacing Rules

SkimSkript has specific spacing rules that differ from many other languages:

1. **Whitespace Significance**: Whitespace is significant for separating tokens but not for indentation.
2. **Parameter Separation**: Function parameters are separated by spaces, not commas.
3. **Multi-word Keywords**: Several keywords consist of multiple words (e.g., `else if`, `is not`).
4. **Reserved Word Phrases**: The Scanner and Evaluator recognize multi-word reserved phrases through a specialized trie data structure.

Example:
```
# Notice spaces between parameters instead of commas
def int Add(int a int b)
{
    return a + b
}

# Multi-word keyword example
if count is not 0
    print("Non-zero value")
```

## 3. Data Types

SkimSkript supports four basic data types: `int`, `float`, `bool`, and `string`.

### A. `int` (Integer)

**Overview**
- **Size**: 4 bytes
- **Description**: Represents whole numbers, both positive and negative.
- **Literals**: Directly adjacent digit characters, optionally preceded by a minus sign.
- **Range**: -2,147,483,648 to 2,147,483,647 (-2^31 to 2^31-1)

**Declaration Examples**
```
int x = 5
declare int y as 10
set x to -42
```

**Supported Mathematical Operations**
- Addition (`+`)
```
int sum = 5 + 3     # sum is 8
```
- Subtraction (`-`)
```
int diff = 10 - 4   # diff is 6
```
- Multiplication (`*`)
```
int product = 3 * 4 # product is 12
```
- Division (`/`)
```
int quotient = 8 / 3 # quotient is 2 (integer division)
```
- Modulus (`%` or `left after dividing by`)
```
int remainder = 8 % 3                  # remainder is 2
int remainder2 = 8 left after dividing by 3  # remainder2 is 2
```
- Exponentiation (`^`)
```
int power = 2 ^ 3   # power is 8
```

**Supported Comparison Operations**
```
# All these comparison expressions evaluate to bool values
5 == 5    # true (equality)
5 is 5    # true (natural language equality)
6 != 7    # true (inequality)
6 is not 7 # true (natural language inequality)
8 > 3     # true (greater than)
8 exceeds 3 # true (natural language greater than)
2 < 10    # true (less than)
2 is below 10 # true (natural language less than)
5 >= 5    # true (greater than or equal)
5 is at least 5 # true (natural language greater than or equal)
7 <= 10   # true (less than or equal)
7 is at most 10 # true (natural language less than or equal)
```

### B. `float` (Floating-Point Number)

**Overview**
- **Size**: 4 bytes
- **Description**: Represents numbers with fractional parts (decimals).
- **Literals**: Digits with a decimal point, optionally preceded by a minus sign.

**Declaration Examples**
```
float pi = 3.14159
declare float e as 2.71828
set pi to 3.14
```

**Supported Mathematical Operations**
```
float sum = 3.5 + 2.1         # sum is 5.6
float diff = 7.8 - 2.3        # diff is 5.5
float product = 2.5 * 3.0     # product is 7.5
float quotient = 10.0 / 3.0   # quotient is 3.33333... (floating-point division)
float remainder = 10.0 % 3.0  # remainder is 1.0 (floating-point modulus)
float power = 2.0 ^ 3.0       # power is 8.0
```

**Supported Comparison Operations**
```
3.14 == 3.14        # true
3.14 is 3.14        # true
3.14 != 3.15        # true
3.14 is not 3.15    # true
5.6 > 2.3           # true
5.6 exceeds 2.3     # true
1.2 < 4.5           # true
1.2 is below 4.5    # true
3.0 >= 3.0          # true
3.0 is at least 3.0 # true
2.5 <= 7.8          # true
2.5 is at most 7.8  # true
```

### C. `bool` (Boolean)

**Overview**
- **Size**: 1 byte
- **Description**: Represents logical values: `true` or `false`.
- **Literals**: The reserved words `true` or `false`.

**Declaration Examples**
```
bool isActive = true
declare bool hasPermission as false
set isActive to false
```

**Supported Logical Operations**
```
true and true       # true (logical AND)
true && true        # true (alternate AND syntax)
true or false       # true (logical OR)
true || false       # true (alternate OR syntax)
true or just false  # true (logical XOR - exclusive OR)
true ^^ false       # true (alternate XOR syntax)
false or just false # false
```

**Supported Comparison Operations**
```
true == true        # true
true is true        # true
true != false       # true
true is not false   # true
```

### D. `string` (String)

**Overview**
- **Size**: Variable; each character is 1 byte.
- **Description**: Represents sequences of characters.
- **Literals**: Unicode characters enclosed by double quotes.

**Declaration Examples**
```
string name = "John"
declare string greeting as "Hello, World!"
set greeting to "Welcome!"
```

**Supported Mathematical Operations**
```
string fullName = "John" + " " + "Doe"  # fullName is "John Doe" (concatenation)
string noVowels = "Hello" - "e"         # noVowels is "Hllo" (character removal)
```

**Supported Comparison Operations**
```
"apple" == "apple"           # true
"apple" is "apple"           # true
"apple" != "orange"          # true
"apple" is not "orange"      # true
"banana" > "apple"           # true (lexicographical comparison)
"banana" exceeds "apple"     # true
"apple" < "banana"           # true
"apple" is below "banana"    # true
"zebra" >= "zebra"           # true
"zebra" is at least "zebra"  # true
"apple" <= "banana"          # true
"apple" is at most "banana"  # true
```

## 4. Variables

Variables in SkimSkript are statically typed, and the data type cannot change after declaration.

### A. Variable Declarations

Both explicit and shorthand styles are supported for variable declarations.

**Explicit Variable Declaration**
```
declare int num as 5
declare float pi as 3.14159
declare string greeting as "Hello"
declare bool isActive as true
```

**Shorthand Variable Declaration**
```
int num = 5
float pi = 3.14159
string greeting = "Hello"
bool isActive = true
```

**Mixed Style Declaration**
```
declare int num = 10       # Uses 'declare' with '='
float pi as 3.14159        # Uses 'as' without 'declare'
```

**Declaration Without Initialization**
```
declare int counter        # Initialized with default value (0)
string name               # Initialized with default value ("")
bool flag                 # Initialized with default value (false)
float temperature         # Initialized with default value (0.0)
```

### B. Variable Assignments

Both explicit and shorthand styles are supported for assigning values to existing variables.

**Explicit Variable Assignment**
```
set num to 42
set greeting to "Welcome"
set isActive to false
set pi to 3.14
```

**Shorthand Variable Assignment**
```
num = 42
greeting = "Welcome"
isActive = false
pi = 3.14
```

**Mixed Style Assignment**
```
set num = 24              # Uses 'set' with '='
greeting to "Hi there"    # Uses 'to' without 'set'
```

**Type Coercion in Assignments**
```
float num = 42            # Integer 42 is coerced to float 42.0
int boolVal = true        # Boolean true is coerced to integer 1
string numStr = 123       # Integer 123 is coerced to string "123"
int strVal = "123"        # String "123" is coerced to integer 123
```

## 5. Expressions

### A. Order of Operations

SkimSkript follows standard operator precedence rules (PEMDAS/BODMAS):

1. **P**arentheses `()`
2. **E**xponents `^`
3. **M**ultiplication `*`, **D**ivision `/`, **M**odulus `%`
4. **A**ddition `+`, **S**ubtraction `-`

```
result = 2 + 3 * 4       # Evaluates to 14 (multiplication before addition)
result2 = (2 + 3) * 4    # Evaluates to 20 (parentheses enforce precedence)
result3 = 2 ^ 3 * 2      # Evaluates to 16 (exponentiation first, then multiplication)
result4 = 10 - 3 + 2     # Evaluates to 9 (left-to-right for same precedence)
result5 = 12 / 4 * 3     # Evaluates to 9 (left-to-right for same precedence)
```

### B. Comparison Operators

These operators return a boolean value (`true` or `false`).

- `==` or `is` (equal to)
- `!=` or `is not` (not equal to)
- `>` or `exceeds` (greater than)
- `<` or `is below` (less than)
- `>=` or `is at least` (greater than or equal to)
- `<=` or `is at most` (less than or equal to)

```
isGreater = 10 > 5                # true
isEqual = (4 + 1) == 5            # true
isValid = 6 <= 3                  # false
hasText = "Hello" is not ""       # true
isSame = 5.0 == 5                 # true (type coercion happens)
isTrue = (3 > 2) == true          # true
```

### C. Logical Operators

- `and` or `&&` – Both conditions must be true.
- `or` or `||` – At least one condition must be true.
- `or just` or `^^` – Exclusive OR (XOR) - One condition must be true, but not both.

```
isValid = (10 > 5) and (3 < 8)                # true (both conditions true)
shouldContinue = (isRunning or hasPermission) # true if either is true
exclusiveCheck = (x == 10) or just (y == 20)  # true if exactly one is true

# Short-circuit evaluation
result = false and SomeFunction()  # SomeFunction is not called (short-circuit)
result = true or SomeFunction()    # SomeFunction is not called (short-circuit)
```

### D. Nested Expressions

Expressions can be nested to create complex conditions and calculations.

```
bool isValid = ((x > 0) and (y < 10)) or ((z == 5) and (w != 0))

int result = ((a + b) * c) ^ (d / e)

string message = "The result is: " + (x + y * z)
```

### E. Function Calls in Expressions

Function calls can be part of expressions. Return values are seamlessly integrated.

```
int total = GetValue() + 5
bool isHighScore = score > GetHighScore()
string greeting = "Hello, " + GetUserName()
```

## 6. Control Structures

### A. If Statements

If statements allow conditional code execution in explicit or shorthand forms.

**Explicit If/ElseIf/Else**
```
if isRunning then
    print "Running"
instead if num == 2 then
    print "Number is 2"
otherwise
    print "Stopped"
```

**Shorthand If/ElseIf/Else**
```
if isRunning
    print "Running"
else if num == 2
    print "Number is 2"
else
    print "Stopped"
```

**Mixed Style If Statements**
```
if isRunning
    print "Running"
instead if num == 2
    print "Number is 2"
else
    print "Stopped"
```

**Nested If Statements**
```
if x > 0
{
    if y > 0
        print "Both x and y are positive"
    else
        print "Only x is positive"
}
else
    print "x is not positive"
```

**Boolean Expression Evaluation**
```
# Non-boolean expressions are implicitly converted to boolean
if count        # Equivalent to (count != 0)
    print "Count is non-zero"

if name         # Equivalent to (name != "")
    print "Name is not empty"
```

### B. While Loops

While loops enable repeated execution as long as a condition is true.

**Explicit While Loop**
```
repeat this while isLooping
    print "Looping"
```

**Shorthand While Loop**
```
while isLooping
    print "Looping"
```

**Loop with Complex Condition**
```
while (x < 10) and (y > 0)
{
    print "x: " + x + ", y: " + y
    x = x + 1
    y = y - 1
}
```

**Infinite Loop with Break Logic**
```
bool running = true
while running
{
    string input = read()
    if input == "quit"
        running = false  # Exit condition
    else
        print "You entered: " + input
}
```

### C. Control Structure Blocks

Control structures support both explicit and implicit blocks.

**Implicit Blocks** (single statement, no curly braces)
```
if userInput is 20
    print("Your input is 20.")

while count is not 3
    count = count + 1
```

**Explicit Blocks** (multiple statements, with curly braces)
```
if userInput is 31 or userInput is 32
{
    print("Your input is 31.")
    print("Or maybe it was 32?")
}

while myScore is below 10
{
    myScore = myScore + 2
    print("Your score: " + myScore)
}
```

**Mixed Block Style**
```
if name is "Greg Jr."
{
    print("Hey Junior!")
    print("Is dad there?")
}
else if name is "Greg"
    print("Hello admin!")
else
    print("Do I know you?")
```

**Multiple Statements in Explicit Blocks**
```
if temperature > 30
{
    print("It's hot today!")
    print("Wear light clothes.")
    print("Stay hydrated.")
}
```

## 7. Functions

### A. Function Definitions

Functions in SkimSkript allow for reusable blocks of code.

**Explicit Function Definition**
```
define int function GetSum(int operand1 int operand2)
{
    return operand1 + operand2
}

define function StoreSum(reference int sumVar int operand1 int operand2)
{
    sumVar = operand1 + operand2
}
```

**Shorthand Function Definition**
```
def SayHello()
{
    print("Hello")
}

def int GetCount()
{
    return 3
}
```

**Function with No Parameters**
```
def bool IsSystemReady()
{
    # Check system status
    return true
}
```

**Function with Multiple Parameters of Different Types**
```
def string FormatUser(string name int age bool isPremium)
{
    string status = isPremium ? "Premium" : "Standard"
    return name + " (" + age + ") - " + status
}
```

### B. Parameters

Parameters can be passed by value or by reference.

**Pass by Value (Default)**
```
def int GetSum(int operand1 int operand2)
{
    # Changes to operand1 and operand2 don't affect the original variables
    operand1 = operand1 * 2  # This modification is local to the function
    return operand1 + operand2
}
```

**Pass by Reference**
```
def StoreSum(ref int sumVar int operand1 int operand2)
{
    # Changes to sumVar affect the original variable
    sumVar = operand1 + operand2
}

# Alternative reference syntax
def StoreSum(reference to int sumVar int operand1 int operand2)
{
    sumVar = operand1 + operand2
}
```

**Multiple Reference Parameters**
```
def SwapValues(ref int a ref int b)
{
    int temp = a
    a = b
    b = temp
}
```

### C. Return Statements

Functions can return values using return statements.

**Explicit Return**
```
give back result

define int function GetValue()
{
    give back 3.5  # The float 3.5 is coerced to int 3
}
```

**Shorthand Return**
```
return result

define float function GetHalf()
{
    return 2  # The integer 2 is coerced to float 2.0
}
```

**Early Returns**
```
def int FindMax(int a int b int c)
{
    if a >= b and a >= c
        return a
    
    if b >= a and b >= c
        return b
    
    return c
}
```

**Void Functions (No Return Value)**
```
def DisplayMenu()
{
    print("1. Start Game")
    print("2. Options")
    print("3. Exit")
    # No return statement needed
}
```

### D. Function Blocks

Function definitions require a block of code that determines the function's behavior.

**Implied Blocks** (single statement)

Shorthand implied block using `=>`:
```
def int GetSum(int operand1 int operand2) =>
    return operand1 + operand2
```

Explicit implied block using `as follows`:
```
def int GetSum(int operand1 int operand2) as follows
    return operand1 + operand2
```

**Explicit Blocks** (multiple statements)
```
def int GetSum(int operand1 int operand2)
{
    int result = operand1 + operand2
    print("Computed sum: " + result)
    return result
}
```

**Nesting Control Structures in Function Blocks**
```
def int ProcessData(int[] data)
{
    int sum = 0
    int i = 0
    
    while i < data.length
    {
        if data[i] > 0
            sum = sum + data[i]
        
        i = i + 1
    }
    
    return sum
}
```

### E. Function Calls

Functions are executed using function calls.

**Explicit Function Call**
```
run PrintHelloWorld()
run GetSum(20 userNumber)
```

**Shorthand Function Call**
```
PrintHelloWorld()
GetSum(20 userNumber)
```

**Pass by Reference in Function Calls**
```
run StoreSum(reference mySum 20 5)  # Explicit
StoreSum(ref mySum 20 5)           # Shorthand
```

**Function Calls in Expressions**
```
total = 20 + value of GetSum(20 userNumber)  # Explicit
total = 20 + GetSum(20 userNumber)          # Shorthand
```

**Nested Function Calls**
```
int result = GetMax(GetValue(x) GetValue(y) GetValue(z))
```

**Function Call with No Arguments**
```
ClearScreen()
string userName = GetUserName()
```

## 8. Standard Library Functions

SkimSkript includes several built-in functions:

### print

Prints values to the console. Can take multiple arguments, each printed on a new line.

```
print("Hello, World!")               # Basic usage
print(name age "Welcome")           # Multiple arguments
print("The result is: " + result)    # Using expressions
print()                             # Just prints a newline
```

### read

Reads a line of input from the user and returns it as a string.

```
string name = read()                 # Basic usage
int age = read()                     # String is auto-coerced to int
print("Enter your name:")
string userName = read()             # Reading after a prompt
```

### clear

Clears the console screen.

```
clear()                              # Clears the entire console
```

## 9. Scope

### A. Top-Level Scope

Variables declared at the top level exist throughout the entire program, from the point of declaration onward.

```
# At this point, isRunning does not exist

bool isRunning = true  # Declared at the top level, accessible everywhere below

# Now isRunning exists and can be used
```

### B. Block Scope

Each control structure introduces a new block scope. Variables declared inside a block are only accessible within that block and nested blocks.

```
while isRunning
{
    int selectedNum = read()  # Exists only inside this while block

    if selectedNum <= 12 && selectedNum > 0
        print(selectedNum + "! = " + Factorial(selectedNum))
    else
        print("Error: Numbers must be less than 13 and greater than 0")

    isRunning = read() == 1  # Can modify global isRunning
}

# selectedNum no longer exists outside the while block
```

### C. Function Scope

Functions have their own isolated scope. Only parameters and top-level variables are accessible inside a function.

```
def int Factorial(int n)
{
    # Can access function parameter (n) and global variables
    if n == 0
        return 1

    return n * Factorial(n - 1)  # Recursive calls work as expected
}

# Variables declared inside Factorial() are not accessible outside
```

### D. Parameter Scope

Parameters exist only within the function scope where they are defined.

```
def ProcessValues(int x int y)
{
    # x and y are accessible here
    print(x + y)
    
    {
        # x and y are also accessible in nested blocks
        int z = x * y
        print(z)
    }
    
    # z is not accessible here
}

# x and y are not accessible here
```

### E. Scoping Rules

1. Variables cannot be used before they are declared.
2. Block-scoped variables disappear after execution of their block.
3. Functions only have access to top-level variables and their parameters.
4. Variables inside a function do not affect variables outside unless passed by reference.

**Variable Shadowing**
```
int x = 10                # Global x

def TestFunction()
{
    int x = 20            # Local x shadows (hides) global x
    print(x)              # Prints 20
    
    {
        int x = 30        # Nested block x shadows function x
        print(x)          # Prints 30
    }
    
    print(x)              # Prints 20 again
}

print(x)                  # Prints 10 (global x)
```

## 10. Type Coercion

SkimSkript supports automatic type coercion in various contexts. Here's how different types are coerced:

### A. Numeric Type Coercion

**Int to Float**
```
float x = 5              # Int 5 is coerced to float 5.0
float y = 2 + 3.5        # Int 2 is coerced to float 2.0, then added
```

**Float to Int**
```
int x = 3.7              # Float 3.7 is coerced to int 3 (truncation)
```

### B. Boolean Type Coercion

**Boolean to Int**
```
int x = true             # Bool true is coerced to int 1
int y = false            # Bool false is coerced to int 0
```

**Boolean to Float**
```
float x = true           # Bool true is coerced to float 1.0
float y = false          # Bool false is coerced to float 0.0
```

**Boolean to String**
```
string x = true          # Bool true is coerced to string "true"
string y = false         # Bool false is coerced to string "false"
```

### C. String Type Coercion

**String to Int**
```
int x = "123"            # String "123" is coerced to int 123
int y = "abc"            # Runtime error - cannot coerce "abc" to int
```

**String to Float**
```
float x = "45.67"        # String "45.67" is coerced to float 45.67
float y = "invalid"      # Runtime error - cannot coerce "invalid" to float
```

**String to Boolean**
```
bool x = "true"          # String "true" is coerced to bool true
bool y = ""              # String "" is coerced to bool false (empty string)
bool z = "anything"      # String "anything" is coerced to bool true (non-empty)
```

### D. Int and Float Type Coercion

**In Expressions**
```
5 + 3.2                  # Int 5 is coerced to float 5.0, result is float 8.2
```

**In Comparisons**
```
5 == 5.0                 # Evaluates to true, int 5 is coerced to float 5.0
```

**In Function Parameters**
```
def PrintFloat(float x)
{
    print(x)
}

PrintFloat(42)           # Int 42 is coerced to float 42.0
```

**In Function Return Values**
```
def float GetValue()
{
    return 5             # Int 5 is coerced to float 5.0
}
```

## 11. Compilation Process

SkimSkript follows a traditional compiler/interpreter pipeline:

### A. Lexical Analysis

The `Lexer` class coordinates the lexical analysis process, which involves:

1. **Scanning**: The `Scanner` class breaks source code into individual lexemes and determines their types.
2. **Tokenization**: The `Evaluator` class processes lexemes and converts them into tokens with specific `TokenType` values.

The lexical analysis process handles:
- Identifier recognition
- Reserved word detection (using a trie data structure)
- Numeric literal parsing
- String literal parsing
- Operator and delimiter identification
- Comment filtering

### B. Syntax Analysis (Parsing)

The `Parser` class converts the token stream into an Abstract Syntax Tree (AST):

1. Parses declarations and expressions
2. Builds structured representations of statements
3. Creates function definitions
4. Constructs control flow structures (if, while, etc.)

The AST is built from various node types:
- `StatementNode` (assignments, declarations, function calls)
- `ExpressionNode` (arithmetic, logical, comparison)
- `ControlStructNode` (if statements, while loops)
- `ValueNode` (literals of different data types)
- `FunctionNode` (function definitions)

### C. Semantic Analysis

The `SemanticAnalyzer` class validates the AST for logical correctness:

1. Verifies variable declarations and references
2. Checks function call arguments against parameters
3. Validates return statements and types
4. Ensures expressions have compatible types
5. Detects unreachable code

### D. Interpretation

The `Interpreter` class executes the AST nodes:

1. Manages variable scopes and values
2. Executes statements an

## 12. Operator Aliases

SkimSkript provides both symbolic and verbose versions of operators for maximum flexibility:

| Symbol | Verbose |
|--------|---------|
| + | plus |
| - | minus |
| * | times, multiplied by |
| / | divided by |
| % | modulus, mod, remainder after dividing by |
| ^ | raised to, to the power of |
| > | exceeds, greater than |
| < | is below, less than |
| == | is |
| != | is not |
| >= | is at least, greater than or equal |
| <= | is at most, less than or equal |
| \|\| | or |
| && | and |
| ^^ | or just |
| = | (assignment operator - no verbose equivalent) |
