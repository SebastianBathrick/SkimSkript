# 2. Lexical Structure
## A. Ignored Characters
> One unique feature of SkimSkript is that the interpreter treats most characters other than alphabetic, operators, or numeric as whitespace (aside from string literals or comments). This leeway gives programmers more freedom to format their code in ways that make sense to them or aid new programmers in moving to more advanced languages.
> For example, if someone still learning programming feels confident with SkimSkript and plans on transitioning to a C-family language, SkimSkript can help. One could practice C-family language syntax in a more forgiving environment using ignored characters like semicolons and commas.
> ```
> Put FizzBuzz sample resembling C
> ```
> Let's say you're already familiar with programming but enjoy Python's syntax; SkimSkript can facilitate many of those conventions, creating a more comfortable programming experience. For example, you could add colons to the end of ifs and while structures to resemble Python.
> ```
> Put FizzBuzz sample resembling Python.
> ```
## B. Comments
> Comments in SkimSkript are inline. They start with the `#` symbol, and everything following it on the same line is ignored by the interpreter.
> ```
> # This is a single-line comment
> print("Hello World") # Inline comment after code
>
> # Multi-line comment example:
> # Each line must start with a `#`
> # to continue the comment.
> ```
 
# 3. Data Types
## A. `int` (Integer)
### Overview
> - **Size**: 4 bytes
> 
> - **Description**: Represents whole numbers, both positive and negative.
> 
> - **Literals**: Directly adjacent digit characters or a single character that make up integers from `-2147483648` to `2147483647`.
>   - ***Note:*** The character prior to the first digit can be a minus sign to indicate the number is negative.
>   - ***Examples:*** `225` is a positive int literal while `-3` is a negative int literal.
> 
### Supported Mathematical Operations
>   - Addition (`+`)
>   - Subtraction (`-`)
>   - Multiplication (`*`)
>   - Division (`/`)
>   - Modulus (`%`)
>   - Exponentiation (`^`)
> 
### Supported Comparison Operations
>    -   Equals (`==` or `is`)
>    -   Not equals (`!=` or `is not`)
>    -   Greater than (`>` or `exceeds`)
>    -   Greater than or equal (`>=` or `is at least`)
>    -   Less than (`<` or `is below`)
>    -   Less than or equal (`<=` or `is at most`)
> -   **Type Coercion in Expressions**:
>      -   Automatically coerced to `float` or `string` when combined with those types in expressions.

## B. `float` (Floating-Point Number)
### Overview
> - **Size**: 4 bytes
> 
> - **Description**: Represents numbers with fractional parts (decimals).
> 
> - **Literals**: Directly adjacent digit characters or a single digit character, directly followed by a single decimal, and ending with directly adjacent digit characters or a single digit character.
>   - ***Note:*** The character prior to the first digit (before the decimal) can be a minus sign to indicate the number is negative.
>   - ***Examples:*** `1.25` is a positive float literal while `-5050.254` is a negative float literal.
> 
### Supported Mathematical Operations
>   - Addition (`+`)
>   - Subtraction (`-`)
>   - Multiplication (`*`)
>   - Division (`/`)
>   - Modulus (`%`)
>   - Exponentiation (`^`)
> 
### Supported Comparison Operations
>    -   Equals (`==` or `is`)
>    -   Not equals (`!=` or `is not`)
>    -   Greater than (`>` or `exceeds`)
>    -   Greater than or equal (`>=` or `is at least`)
>    -   Less than (`<` or `is below`)
>    -   Less than or equal (`<=` or `is at most`)
> -   **Type Coercion in Expressions**:
>      -   Automatically coerced to `string` when combined with `string` in expressions.

## C. `bool` (Boolean)
### Overview
> - **Size**: 1 byte
> 
> - **Description**: Represents logical values: `true` or `false`.
> 
> - **Literals**: Simply the reserved words `true` or `false`.
> 
### Supported Logical Operations
>   - And (`and` or `&&`)
>   - Or (`or` or `||`)
>   - XOR (`or just` or `^^`)
> 
### Supported Comparison Operations
>    -   Equals (`==` or `is`)
>    -   Not equals (`!=` or `is not`)
> 
> - **Type Coercion in Expressions**:
>      -   Automatically coerced to `int`, `float`, or `string` when combined with those types in expressions.

## D. `string` (String)
### Overview
> - **Size**: Variable; each character is 1 byte.
> 
> - **Description**: Represents sequences of characters.
> 
> - **Literals**: Single, series, or no Unicode characters enclosed by double quotes.
>   - ***Note:*** The only exception for characters within the double quotes are double quotes themselves.
>   - ***Examples:*** `"This is an example!"` would be a valid string along with `"My phone number is +1 913-258-3245"`.
> 
### Supported Mathematical Operations
>   - Addition (`+`): Concatenates two strings.
>   - Subtraction (`-`): Removes occurrences of the right string from the left string.
> 
### Supported Comparison Operations
>    -   Equals (`==` or `is`): Checks if two strings are identical.
>    -   Not equals (`!=` or `is not`): Checks if two strings are different.
>    -   Greater than (`>` or `exceeds`): Checks if the left string is lexicographically greater than the right.
>    -   Greater than or equal (`>=` or `is at least`): Checks if the left string is lexicographically greater than or equal to the right.
>    -   Less than (`<` or `is below`): Checks if the left string is lexicographically less than the right.
>    -   Less than or equal (`<=` or `is at most`): Checks if the left string is lexicographically less than or equal to the right.
> -   **Type Coercion in Expressions**:
>      -   Does not automatically coerce to other types in expressions.


 
# 4. Variables
## A. Variable Declarations

> Variables in SkimSkript are statically typed, and the data type cannot change after declaration. Both explicit and shorthand styles are supported.
> ### Explicit Variable Declaration
> Explicit declarations use the keyword `declare` followed by the type, identifier, and an optional initializer using `as` along with an expression. Additional rules for initializations can be found in the next section discussing variable assignments.
>```
># Explicit examples:
>declare int num as 5
>declare float num2 as 3.14
>declare string greeting
>```
>### Shorthand Variable Declaration
>Shorthand declarations omit the `declare` keyword and use `=` for initialization.
>```
># Shorthand examples:
>int num = 5
>float num2 = 3.14
>string greeting
>```
## Mix & Match Declarations
>The syntax allows combining explicit and shorthand elements.
>```
># Mixed examples:
>declare int num = 10
>float num2 as 2.71
>```
## B. <u>Variable Assignments
>Both styles allow assigning a value to an existing variable. The assignment must follow data type rules; however, SkimSkript supports type coercion if the data types are compatible.
>### Explicit Variable Assignment
>Explicit assignments use the keyword `set` and the phrase `to`.
>```
># Explicit example:
>set num to 42
>```
>### Shorthand Variable Assignment
>Shorthand assignments directly use `=`.
>```
># Shorthand example:
>num = 42
>```
>### Mix & Match Assignments
>Assignments can mix the explicit and shorthand styles.
>```
># Mixed examples:
>set num = 24
>num to 36
>```
>### Type Coercion Behavior
>If an expression's type can be coerced to match the variable's type, the assignment will succeed. For example:
>```
># Type coercion examples:
>float num = 42  # Integer 42 is coerced to float 42.0
>int num2 = false # Boolean false is coerced to 0
>```
>If coercion is not possible, SkimSkript raises a runtime error. More detailed documentation regarding coercion will be coming soon.
# 5. Expressions
## A. Order of Operations
>SkimSkript respects the conventional precedence rules for arithmetic operations:
>1.  Parentheses `()` – Expressions inside parentheses are evaluated first.
>2.  Exponents `^` – Exponentiation is processed before multiplication and division.
>3.  Multiplication `*`, Division `/` – Thse operations are evaluated before addition and subtracetion.
>4.  Addition `+`, Subtraction `-` – These are evaluated last in standard arithmetic.
>### Examples:
>```
>result = 2 + 3 * 4  # Evaluates to 14 (multiplication happens before addition)
>result2 = (2 + 3) * 4  # Evaluates to 20 (parentheses enforce precedence)
>result3 = 2 ^ 3 * 2  # Evaluates to 16 (exponentiation happens first: 2^3 = 8, then 8*2)
>```
## B. Comparison Operators
>SkimSkript also supports comparison operators, which allow expressions to be used in conditional statements.
>### Available Comparison Operators:
>-   `==` or `is` (equal to)
>-   `!=` or `is not` (not equal to)
>-   `>` or `exceeds` (greater than)
>-   `<` or `is below` (less than)
>-   `>=` or `is at least` (greater than or equal to)
>-   `<=` or `is at most` (less than or equal to)
>These operators return a boolean value (`true` or `false`).
>### Examples:
>```
>isGreater = 10 > 5  # Evaluates to true
>isEqual = (4 + 1) == 5  # Evaluates to true
>isValid = 6 <= 3  # Evaluates to false
>```
## C. Logical Operators
>Logical expressions can combine boolean values using logical operators:
>-   `and` or `&&` – Both conditions must be true.
>-   `or` or `||` – At least one condition must be true.
>-   `or just` or `^^` – One condition must be true, but not both.
>### Examples:
>```
>isValid = (10 > 5) and (3 < 8)  # Evaluates to true
>shouldContinue = (isRunning or hasPermission)  # Either condition being true results in true
>exclusiveCheck = (x == 10) or just (y == 20)  # Only one of these can be true
>```
## D. Execution Order in Expressions
>Internally, expressions are parsed and executed based on their precedence levels, with a maximum recursion depth of 4 for a single expression:
>1.  Logical expressions (lowest precedence)
>2.  Comparison expressions
>3.  Arithmetic expressions (following PEMDAS/BODMAS order)
>4.  Parentheses enforce manual grouping
>
>This ensures that all expressions are evaluated in a logical, structured manner.
# 6. Control Structures
## If Statements

If statements in SkimSkript allow conditional code execution in explicit or shorthand forms.

### Explicit If/ElseIf/Else
Explicit syntax uses `if`, `but if` (for `else if`), and `otherwise` (for `else`).
```
# Explicit example:
if isRunning then
    print "Running"
but if num == 2 then
    print "Number is 2"
otherwise
    print "Stopped"
```
### Shorthand If/ElseIf/Else
The shorthand syntax omits keywords like `then` and uses `else if` and `else`.
```
# Shorthand example:
if isRunning
    print "Running"
else if num == 2
    print "Number is 2"
else
    print "Stopped"
```
### Mix & Match If Statements
Elements of explicit and shorthand syntax can be combined.
```
# Mixed example:
if isRunning
    print "Running"
but if num == 2
    print "Number is 2"
else
    print "Stopped"
```
## B. While Loops
>While loops enable repeated execution while a condition is true.
>### Explicit While
>Explicit while loops use `repeat this while` before the condition.
>```
># Explicit example:
>repeat this while isLooping
>   print "Looping"
>```
>### Shorthand While
>Shorthand while loops omit `repeat this while`.
>```
># Shorthand example:
>while isLooping
>    print "Looping"
>```
### C. Control Structure Blocks
>Control structures support both explicit and implicit blocks. Explicit blocks are enclosed in curly braces and can contain more than one statement. Implicit blocks, on the other hand, do not require those curly braces but can contain no more than a single statement.
>## If Statement Blocks
>If statements using implicit blocks look like this:
>```
># Implicit block example:
>if userInput is 20
>print("Your input is 20.")
>```
>If statements using explicit blocks can be written like the following:
>```
># Implicit block example:
>if userInput is 31 or userInput is 32
>{
>	print("Your input is 31.")
>	print("Or maybe it was 32?")
>}
>```
>However, connected if, else if, and elses can all be using different block types.
>```
># Implicit & explicit mix example:
>if name is "Greg Jr."
>{
>	print("Hey Junior!")
>	print("Is dad there?")
>}
>else if name is "Greg"
>	print("Hello admin!)
>else
>	print("Do I know you?")
>```
>
>## While Loop Blocks
>While loops using implicit blocks look like this:
>```
># Implicit block example:
>while count is not 3
>	count = count + 1
>```
>While loops using explicit blocks can be written like the following:
>```
># Explicit block example:
>while myScore is below 10
>{
>	myScore = myScore + 2
>	print("Your score: " + myScore)
>}
>```
