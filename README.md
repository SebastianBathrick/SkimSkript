# 2. Lexical Structure
## A. Ignored Characters
One unique feature of SkimSkript is that the interpreter treats most characters other than alphabetic, operators, or numeric as whitespace (aside from string literals or comments). This leeway gives programmers more freedom to format their code in ways that make sense to them or aid new programmers in moving to more advanced languages.

For example, if someone still learning programming feels confident with SkimSkript and plans on transitioning to a C-family language, SkimSkript can help. One could practice C-family language syntax in a more forgiving environment using ignored characters like semicolons and commas.
```
Put FizzBuzz sample resembling C
```
Let's say you're already familiar with programming but enjoy Python's syntax; SkimSkript can facilitate many of those conventions, creating a more comfortable programming experience. For example, you could add colons to the end of ifs and while structures to resemble Python.
```
Put FizzBuzz sample resembling Python.
```

## B. Comments

Comments in SkimSkript are inline. They start with the `#` symbol, and everything following it on the same line is ignored by the interpreter.

```
# This is a single-line comment
print("Hello World") # Inline comment after code

# Multi-line comment example:
# Each line must start with a `#`
# to continue the comment.
```
 
# 3. Data Types
## A. `int` (Integer)

-   **Size**: 4 bytes
-   **Description**: Represents whole numbers, both positive and negative.
-  **Literals**: Directly adjacent digit characters or a single character that make up integers from `-2147483648` to `2147483647`. 
	- Note: The character prior to the first digit can be a minus sign to indicate the number is negative.
	- Examples: `225` is a positive int literal while `-3` is a negative int  literal.
-   **Supported Mathematical Operations**:
    -   Addition (`+`)
    -   Subtraction (`-`)
    -   Multiplication (`*`)
    -   Division (`/`)
    -   Modulus (`%`)
    -   Exponentiation (`^`)
-   **Supported Comparison Operations**:
    -   Equals (`==` or `is`)
    -   Not equals (`!=` or `is not`)
    -   Greater than (`>` or `exceeds`)
    -   Greater than or equal (`>=` or `is at least`)
    -   Less than (`<` or `is below`)
    -   Less than or equal (`<=` or `is at most`)
-   **Type Coercion in Expressions**:
    -   Automatically coerced to `float` or `string` when combined with those types in expressions.
  
## B. `float` (Floating-Point Number)

-   **Size**: 4 bytes
-   **Description**: Represents numbers with fractional parts (decimals).
- **Literals**: Directly adjacent digit characters or a single digit character, directly followed by a single decimal, and ending with directly adjacent digit characters or a single digit character.
	- Note: The character prior to the first digit (before the decimal) can be a minus sign to indicate the number is negative.
	- Examples: `1.25` is a positive float literal while `-5050.254` is a negative int  literal.
-   **Supported Mathematical Operations**:
    -   Addition (`+`)
    -   Subtraction (`-`)
    -   Multiplication (`*`)
    -   Division (`/`)
    -   Modulus (`%`)
    -   Exponentiation (`^`)
-   **Supported Comparison Operations**:
    -   Equals (`==` or `is`)
    -   Not equals (`!=` or `is not`)
    -   Greater than (`>` or `exceeds`)
    -   Greater than or equal (`>=` or `is at least`)
    -   Less than (`<` or `is below`)
    -   Less than or equal (`<=` or `is at most`)
-   **Type Coercion in Expressions**:
    -   Automatically coerced to `string` when combined with `string` in expressions.

 
## C. `bool` (Boolean)

-   **Size:** 1 byte
-   **Description:** Represents logical values: `true` or `false`.
- **Literals**: Simply the reserved words `true` or `false`.
-   **Supported Logical Operations:**
	   -  And (`and` or `&&`)
	-   Or (`or` or `||`)
	-   XOR (`or just` or `^^`)
-   **Supported Comparison Operations:**    
    -   Equals (`==` or `is`)
    -   Not equals (`!=` or `is not`)
-   **Type Coercion in Expressions:** Automatically coerced to `int`, `float`, or `string` when combined with those types in expressions.
 
## D. `string` (String)

-   **Size**: Variable; each character is 1 byte.
-   **Description**: Represents sequences of characters.
- **Literals**: Single, series, or no Unicode characters enclosed by double quotes.
	- Note: The only exception for characters within the double quotes are double quotes themselves.
	- Examples: `"This is an example!"` would be a valid string along with `"My phone number is +1 913-258-3245"`.
-   **Supported Mathematical Operations**:
    -   Addition (`+`): Concatenates two strings.
    -   Subtraction (`-`): Removes occurrences of the right string from the left string.
-   **Supported Comparison Operations**:
    -   Equals (`==` or `is`): Checks if two strings are identical.
    -   Not equals (`!=` or `is not`): Checks if two strings are different.
    -   Greater than (`>` or `exceeds`): Checks if the left string is lexicographically greater than the right.
    -   Greater than or equal (`>=` or `is at least`): Checks if the left string is lexicographically greater than or equal to the right.
    -   Less than (`<` or `is below`): Checks if the left string is lexicographically less than the right.
    -   Less than or equal (`<=` or `is at most`): Checks if the left string is lexicographically less than or equal to the right.
-   **Type Coercion in Expressions**:
    -   Does not automatically coerce to other types in expressions.
 
# 4. Variables
## A. Variable Declarations

Variables in SkimSkript are statically typed, and the data type cannot change after declaration. Both explicit and shorthand styles are supported.

### Explicit Variable Declaration

Explicit declarations use the keyword `declare` followed by the type, identifier, and an optional initializer using `as` along with an expression. Additional rules for initializations can be found in the next section discussing variable assignments.

```
# Explicit examples:
declare int num as 5
declare float num2 as 3.14
declare string greeting
```

### Shorthand Variable Declaration

Shorthand declarations omit the `declare` keyword and use `=` for initialization.

```
# Shorthand examples:
int num = 5
float num2 = 3.14
string greeting
```

### Mix & Match Declarations

The syntax allows combining explicit and shorthand elements.

```
# Mixed examples:
declare int num = 10
float num2 as 2.71
```
 
## B. <u>Variable Assignments
 
Both styles allow assigning a value to an existing variable. The assignment must follow data type rules; however, SkimSkript supports type coercion if the data types are compatible.

### Explicit Variable Assignment

Explicit assignments use the keyword `set` and the phrase `to`.

```
# Explicit example:
set num to 42
```

### Shorthand Variable Assignment

Shorthand assignments directly use `=`.

```
# Shorthand example:
num = 42
```

### Mix & Match Assignments

Assignments can mix the explicit and shorthand styles.

```
# Mixed examples:
set num = 24
num to 36
```

### Type Coercion Behavior

If an expression's type can be coerced to match the variable's type, the assignment will succeed. For example:

```
# Type coercion examples:
float num = 42  # Integer 42 is coerced to float 42.0
int num2 = false # Boolean false is coerced to 0
```

If coercion is not possible, SkimSkript raises a runtime error. More detailed documentation regarding coercion will be coming soon.
