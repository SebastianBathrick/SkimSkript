# Lexical Structure
## Ignored Characters
One unique feature of SkimSkript is that the interpreter treats most characters other than alphabetic, operators, or numeric as whitespace (aside from string literals or comments). This leeway gives programmers more freedom to format their code in ways that make sense to them or aid new programmers in moving to more advanced languages.

For example, if someone still learning programming feels confident with SkimSkript and plans on transitioning to a C-family language, SkimSkript can help. One could practice C-family language syntax in a more forgiving environment using ignored characters like semicolons and commas.
```
Put FizzBuzz sample resembling C
```
Let's say you're already familiar with programming but enjoy Python's syntax; SkimSkript can facilitate many of those conventions, creating a more comfortable programming experience. For example, you could add colons to the end of ifs and while structures to resemble Python.
```
Put FizzBuzz sample resembling Python.
```
----------
## Comments

Comments in SkimSkript are inline. They start with the `#` symbol, and everything following it on the same line is ignored by the interpreter.

```
# This is a single-line comment
print("Hello World") # Inline comment after code

# Multi-line comment example:
# Each line must start with a `#`
# to continue the comment.
```
___
