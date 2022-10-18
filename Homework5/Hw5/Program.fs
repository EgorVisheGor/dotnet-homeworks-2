open System
open Hw5

let succesPrint(val1, operation, val2) =
    printfn $"{Calculator.calculate val1 operation val2}"
let main (args : string[]) =
    match Parser.parseCalcArguments args with
    | Ok res -> succesPrint res
    | Error message -> printf $"{message}" 