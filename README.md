# What's SkimSkript?

SkimSkript is a programming language that lets you write pseudocode-like code with flexible syntax designed for a broad audience. Each statement in SkimSkript is available in two flavors: explicit and shorthand. Explicit statements use extra keywords or keywords instead of symbols, providing readers with additional context about what the code does, which is perfect for beginners or educators. On the other hand, shorthand statements resemble familiar coding styles, catering to experienced programmers seeking brevity.

## Shorthand vs Explicit
You can see the difference with a simple variable assignment. Suppose we want to store the whole number '3' in a previously declared variable named 'myVar.' 

The **shorthand** assignment statement to represent that looks like this:
````
myVar = 3					
````

Meanwhile, the **explicit** form would add the "*set*"  keyword before the variable identifier and replace the assignment operator with the "*as*" keyword. In practice, the example above in its explicit form would be written like this:
````
set myVar to 3					
````

And if neither style suits your preferences, you can **mix and match**, writing assignments like ````myVar to 3				```` or ````set myVar = 3````. The choice is yours!

## Whitespace Freedom
While keywords and numeric literals must be separated by at least one space or ignored character (we discuss those in the next section), beyond that, the way code is spaced and how line breaks are used is up to the programmer. 

So, a program that looks like this:
````
print("20! = " + Factorial(20))

def int Factorial(int n)
{
    if n == 0
        return 1

    return n * Factorial(n - 1)
}
````
Also could look like this:
````
print ("20! = " + Factorial (20))

def int Factorial (int n)
{
    if n == 0 return 1 return n * Factorial(n - 1)
}
````
Or even this:
````
print ("20! = " + Factorial(20)) def int Factorial(int n) 
{ if n == 0 return 1        return n * Factorial(n - 1) }
````
