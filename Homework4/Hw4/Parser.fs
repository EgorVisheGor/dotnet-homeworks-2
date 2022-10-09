module Hw4.Parser

open System
open Hw4.Calculator


type CalcOptions =
    { arg1: float
      arg2: float
      operation: CalculatorOperation }

let isArgLengthSupported (args: string []) = args.Length = 3

let parseOperation (arg: string) =
    match arg with
    | "+" -> CalculatorOperation.Plus
    | "-" -> CalculatorOperation.Minus
    | "*" -> CalculatorOperation.Multiply
    | "/" -> CalculatorOperation.Divide
    | _ -> raise (ArgumentException("Введенная операция не распознается"))
    
let parseDouble (str: string) =
    match Double.TryParse str with
    | true, str -> str
    | _ -> raise (ArgumentException"Элемент не является числом")

let parseCalcArguments (args: string []) =
    if not (isArgLengthSupported args) then
        raise (ArgumentException("Введенные данные некорректны"))

    let val1 = parseDouble args[0]     
    let operation = parseOperation args[1]
    let val2 = parseDouble args[2] 
    
    {
        arg1 = val1
        operation = operation
        arg2 = val2
    }