# 2. Lexical Structure
## 2a. Ignored Characters
One unique feature of SkimSkript is that the interpreter treats most characters other than alphabetic, operators, or numeric as whitespace (aside from string literals or comments). This leeway gives programmers more freedom to format their code in ways that make sense to them or aid new programmers in moving to more advanced languages.

For example, if someone still learning programming feels confident with SkimSkript and plans on transitioning to a C-family language, SkimSkript can help. One could practice C-family language syntax in a more forgiving environment using ignored characters like semicolons and commas.
```
Put FizzBuzz sample resembling C
```
Let's say you're already familiar with programming but enjoy Python's syntax; SkimSkript can facilitate many of those conventions, creating a more comfortable programming experience. For example, you could add colons to the end of ifs and while structures to resemble Python.
```
Put FizzBuzz sample resembling Python.
```

## 2b. Comments

Comments in SkimSkript are inline. They start with the `#` symbol, and everything following it on the same line is ignored by the interpreter.

```
# This is a single-line comment
print("Hello World") # Inline comment after code

# Multi-line comment example:
# Each line must start with a `#`
# to continue the comment.
```
___
# 3. Data Types

## 3a. `int` (Integer)

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
 
## 3b. `float` (Floating-Point Number)

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


## 3c. `bool` (Boolean)

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

----------

## 3d. `string` (String)

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
