function main() 
{
    print(fibonacci(38))
}

function fibonacci(num: int): int
{
    var a = 1
    var b = 0
    var temp = 0
    var nnum = num

    while nnum >= 0
    {
        temp = a
        a += b
        b = temp
        nnum -= 1
    }
    
    return b
}