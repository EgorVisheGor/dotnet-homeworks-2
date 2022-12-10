module Hw6.Parser

open System
open System.Diagnostics.CodeAnalysis
open System.Globalization
open Hw6.Calculator
open Hw6.Message


[<ExcludeFromCodeCoverage>]
let isArgLengthSupported (args:string[]): Result<_, _> =
    match args.Length with
    | 3 -> Ok args
    | _ -> Error Message.WrongArgLength 
    
[<ExcludeFromCodeCoverage>]
let inline isOperationSupported (arg1, operation, arg2): Result<('a * CalculatorOperation * 'b), Message> =
    match operation with
    | "Plus" -> Ok (arg1, CalculatorOperation.Plus, arg2)
    | "Minus" -> Ok (arg1, CalculatorOperation.Minus, arg2)
    | "Multiply" -> Ok (arg1, CalculatorOperation.Multiply, arg2)
    | "Divide" -> Ok (arg1, CalculatorOperation.Divide, arg2)
    | _ -> Error Message.WrongArgFormatOperation

let parseArgs (args: string[]): Result<('a * CalculatorOperation * 'b), Message> =
   match Double.TryParse(args[0], NumberStyles.Float, CultureInfo.InvariantCulture) with
   | true, arg1  -> match Double.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture) with
                    |true, arg2 -> isOperationSupported (arg1, args[1], arg2)
                    |_ -> Error Message.WrongArgFormatForValue2
   |_ -> Error Message.WrongArgFormatForValue1

[<ExcludeFromCodeCoverage>]
let inline isDividingByZero (arg1 : ^a, operation, arg2 : ^a): Result<('a * CalculatorOperation * 'b),Message> =
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