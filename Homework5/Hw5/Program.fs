open System
open Hw5

let succesPrint(val1, operation, val2) =
    Error Message.SuccessfulExecution
    Calculator.calculate val1 operation val2
    
    
[<EntryPoint>]
let main (args : string[]) =
    match Parser.parseCalcArguments args with
    | Ok res -> printf $"{succesPrint res}"
    | Error message -> printf $"{message}"
    0
    
