module Hw5.Parser

open System
open Hw5.Calculator

let isArgLengthSupported (args:string[]): Result<_, _> =
    match args.Length with
    | 3 -> Ok args
    | _ -> Error Message.WrongArgLength 
    
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | "+" -> Ok (arg1, CalculatorOperation.Plus, arg2)
    | "-" -> Ok (arg1, CalculatorOperation.Minus, arg2)
    | "*" -> Ok (arg1, CalculatorOperation.Multiply, arg2)
    | "/" -> Ok (arg1, CalculatorOperation.Divide, arg2)
    | _ -> Error Message.WrongArgFormatOperation

let parseArgs (args: string[]): Result<('a * CalculatorOperation * 'b), Message> =
   match (Double.TryParse(args[0]), Double.TryParse(args[2])) with
   | (true, arg1), (true, arg2) -> isOperationSupported (arg1, args[1], arg2)
   | _ -> Error Message.WrongArgFormat

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let inline isDividingByZero (arg1 : ^a, operation, arg2 : ^a): Result<_,_> =
    match operation, float arg2 with
    | CalculatorOperation.Divide, 0.0 -> Error Message.DivideByZero
    | _ -> Ok (arg1, operation, arg2)
    
let parseCalcArguments (args: string[]): Result<'a, 'b> =
    MaybeBuilder.maybe{
        let! lengthSupported = isArgLengthSupported args
        let! parsedArgs = parseArgs lengthSupported
        let! result = isDividingByZero parsedArgs
        return result
    }   