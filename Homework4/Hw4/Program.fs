open Hw4

let main args =
    let example = Parser.parseCalcArguments args
    printf $"{Calculator.calculate example.arg1 example.operation example.arg2}"