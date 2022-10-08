module Hw4.Parser

open System
open Hw4.Calculator


type CalcOptions =
    { arg1: float
      arg2: float
      operation: CalculatorOperation }

let isArgLengthSupported (args: string []) =
    match args.Length with
    | 3 -> true
    | _ -> false

let parseOperation (arg: string) =
    match arg with
    | "+" -> CalculatorOperation.Plus
    | "-" -> CalculatorOperation.Minus
    | "*" -> CalculatorOperation.Multiply
    | "/" -> CalculatorOperation.Divide
    | _ -> raise (ArgumentException("Введенная операция не распознается"))

let parseCalcArguments (args: string []) =
    if not (isArgLengthSupported args) then
        raise (ArgumentException("Введенные данные некорректны"))

    let val1 = match System.Double.TryParse args[0] with
      | true, a -> a
      | _ -> raise (ArgumentException "Введенные данные некорректны")
      
    let operation = parseOperation args[1]
    
    let val2 = (match System.Double.TryParse args[2] with
      | true, a -> a
      | _ -> raise (ArgumentException "Введенные данные некорректны"))
    
    {
        arg1 = val1
        operation = operation
        arg2 = val2
    }