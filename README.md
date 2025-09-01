## What is SkimSkript?
SkimSkript is an experimental interpreted language meant to explore different syntactical possibilities. It features flexible, forgiving syntax and enforced typing with runtime coercion, allowing users to write in C-style braces, Python-like keywords, or even natural-language pseudocode. SkimSkript adapts to the programmer — not the other way around.

With support for multiple syntactic styles, programmers can experiment with static types, recursion, and procedural logic without being boxed into rigid syntax rules or overwhelmed by compiler errors.

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
def int factorial(int n):
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
## What's Done Differently?
The SkimSkript interpreter utilizes no third-party libraries, so while it possesses the typical lexical analysis, AST parsing, and interpretation that many other tree interpreters have, it also has some quirks that enable its unique syntactic style.

### How Lexeme Analysis Works
A prime example of this occurs during lexical analysis. After scanning the source code and creating lexemes, the lexer's evaluator feeds alphabetic lexemes into a trie structure. If a single alphabetic lexeme is a partial match for a phrase, the next lexeme is analyzed to check if it is part of that phrase as well. If a group of lexemes reaches a trie node containing a token type, those lexemes are grouped as a single token (assuming the following word does not continue a phrase). However, if a partial match does not result in a full match with subsequent lexemes, the system will backtrack, mark the first lexeme as an identifier, and initiate a new trie search with the second lexeme.

1. _**First Search (at Trie Root):**_
> 
>"instead" (**Partial Match**) --> "else" (**STOP Invalid Phrase**) --> "if" (**Not Reached**)
> 
2. _**Store First Lexeme:**_
> 
>"instead" becomes **Token of type Identifier**
> 
2. _**Second Search (at Trie Root):**_
>
>"else" (**Partial Match**) --> "If" (**Full Match**)
> 
3. _**Store Full Phrase:**_
> 
>"else" & "if" becomes **Token of type ElseIf**

## Architecture Overview
### Lexer
Encapsulates, abstracts, and performs the lexical analysis previously mentioned to convert source code to tokens for parsing.
### Parser
Utilizes tokens to build an abstract syntax tree (AST) for interpretation.
### Interpreter
Handles the execution of the program recursively traversing the AST to do so. Examples of what this component is responsible for include scope, coercion, and evaluating expressions, among others.
## Diagram
<img width="1520" height="637" alt="InterpreterDesign (6)" src="https://github.com/user-attachments/assets/fa7b5a69-53de-417a-8867-2a8c6d82acc1" />
