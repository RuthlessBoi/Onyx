# Design of Onyx

Most of these features are implemented either fully or partially.

If a feature/bullet point is prefiex with a `X`, it isn't completed or implemented

## Types

##### Public types
 - `any` - a universal object type
 - `char` - a single character type
 - `string` - a wrapper around a list of `char`'s
 - `number` - a number type
 - `bool` - a `true` or `false` value type
 - X - `union` as `T | ...` where `...` is a `type` - a unification of multiple data types
 - `T[]` - an array of items of type `T`
 - X - `T?` - an optional value of type `T`
 - `T` where `T` is declared by `template <T> ...` - a user-defined data type container
 - X - `Function[<T...>]` - a type for encapsulation of functions or lambdas

##### Internal types
 - `void` - a zero-return type; like any C-like language
 - `error` as `?` - a type returned by an errored expression

##### Built-in types
 - X - `List<T>` - a list of items of type `T`
 - X - `Map<K, V>` - a list of value of type `V` owned by values of type `K`; uses a key-value-pair system


## Syntax

###### Info
 - a word enclosed by `<` and `>` entail a required word; doubling them negates this effect
 - a word block enclosed by `[` and `]` entail an optional word; a optional block can contain required ones
 - a word block enclosed by `...` on each side, entail either an expression or an any-use token
 - a word followed by `...` entails the word type can repeat
 - a static `...` token entails any previously described syntax applies
 - a word block enclosed by `^` on each side, entails a previous description of the keyword

##### Keywords
 - `let` - declares a constant variable
    - usage: `let <NAME>: <T> = <VALUE>`
 - `var` - declares a dynamic variable
    - usage: `var <NAME>: <T> [= <VALUE>]`
 - `true` as type `bool` - declares a static type of `true` as a `bool` value
    - usage: `... true ...`
 - `false` as type `bool`- declares a static type of `false` as a `bool` value
    - usage: `... false ...`
 - `function` - declares a function along with its paramters
    - usage: `function <NAME>([<PARAMETER>: <T>]...)[: <T>] { ... }` requires declaration inside of a `global scope` or `namespace scope`
 - `return` - declares a return statement within the function block
    - usage: `^function^ { ... return <VALUE> ... }`
 - `if` - declares an if statement for conditional checks
    - usage: `if (<CONDITION>) { ... }`
 - `else` - declares an if-else statement if a condition is false
    - usage: `^if^ { ... } else { ... }`
 - `to` - declares a conversion expressiong; converts `IN` to `OUT`
    - usage: `... <FROM> to <TO>`
 - `do` - declares a do loop
    - usage: `do { ... } while (<CONDITION>)`
 - `is` - declares an is statement; checks if `IN` is of type `EXPECTED`
    - usage: `<IN> is typeof <EXPECTED> [<OUT>]`
 - `for` - declares a for loop
    - usage: `unknown`
 - `while` - declares a while loop
    - usage: `while (<CONDITION>) { ... }`
 - `break` - declares a break statement within a loop body
    - usage: `... break ...` requires declaration inside of a `loop body`
 - `continue` - declares a continue statement within a loop body
    - usage: `... continue ...` requires declaration inside of a `loop body`
 - `namespace` - declares a namespace member
    - usage: `namespace <NAME> { [function|template|class]... }`
 - `template` - declares a template declaration
    - usage: `template <NAME>[<<T>> [: <O>]...] { [<NAME>: <T>]... }` requires declaration inside of a `global scope` or `namespace scope`
 - X - `class` - declares a class declaration
    - usage: `class <NAME>[<<T>> [: <O>]...] { [function|variable]... }` requires declaration inside of a `global scope` or `namespace scope`
 - `new` - declares a new instance expression
    - usage: 
      - Class: `... new <T>[<T>...]([<PARAMETER>]...)` requires `T` is declared by `class`
      - Template: `... new <T>[<T>...] { [<NAME> = <VALUE>]... }` requires `T` is declared by `template`

##### Operators
###### Unary operators
 - `+` - `... +<VALUE> ...` produces `number`
 - `-` - `... -<VALUE> ...` produces `number`
 - `++` - `... ++<VALUE> ...` produces `number`
 - `--` - `... --<VALUE> ...` produces `number`
 - `!` - `... !<VALUE> ...` produces `bool`

###### Binary operators
 - `+` - `... <VALUE1> + <VALUE2> ...` produces `number | string`
 - `-` - `... <VALUE1> - <VALUE2> ...` produces `number`
 - `/` - `... <VALUE1> / <VALUE2> ...` produces `number`
 - `*` - `... <VALUE1> * <VALUE2> ...` produces `number`
 - `%` - `... <VALUE1> % <VALUE2> ...` produces `number`
 - `==` - `... <VALUE1> == <VALUE2> ...` produces `bool`
 - `!=` - `... <VALUE1> != <VALUE2> ...` produces `bool`
 - `&&` - `... <VALUE1> && <VALUE2> ...` produces `bool`
 - `||` - `... <VALUE1> || <VALUE2> ...` produces `bool`
 - `>` - `... <VALUE1> > <VALUE2> ...` produces `bool`
 - `<` - `... <VALUE1> < <VALUE2> ...` produces `bool`
 - `>=` - `... <VALUE1> >= <VALUE2> ...` produces `bool`
 - `<=` - `... <VALUE1> <= <VALUE2> ...` produces `bool`
 - X - `??` - `... <VALUE1> ?? <VALUE2> ...` produces `any`
 - X - `?:` - `... <CONDITION> ? <VALUE1> : <VALUE2> ...` produces `any`

###### Assignment operators
 - `=` - `<VAR> = <VALUE>`
 - `+=` - `<VAR> += <VALUE>`
 - `-=` - `<VAR> -= <VALUE>`
 - `/=` - `<VAR> /= <VALUE>`
 - `*=` - `<VAR> *= <VALUE>`
 - `%=` - `<VAR> %= <VALUE>`