module Hw6.App

open System
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Hw6.Message

[<CLIMutable>]
type Values =
    {
        value1: string
        operation: string
        value2: string
    }
    
    
[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
let calculatorHandler: HttpHandler =
    fun next ctx ->
        
        let values = ctx.BindQueryString<Values>()
        let args = [|values.value1;values.operation;values.value2|]
        let parsedargs = Parser.parseCalcArguments args
        
        let result =
            match parsedargs with
            | Ok ok -> Ok (ok |||> Calculator.calculate)
            | Error error -> Error error
        let message = convertMessageToString(result, args)
        
        match message with
        | Ok ok -> (setStatusCode 200 >=> text (ok.ToString())) next ctx
        | Error error -> (setStatusCode 400 >=> text error) next ctx

let webApp =
    choose [
        GET >=> choose [
             route "/" >=> text "Use //calculate?value1=<VAL1>&operation=<OPERATION>&value2=<VAL2>"
             route "/calculate" >=> calculatorHandler
        ]
        setStatusCode 404 >=> text "Not Found" 
    ]
    
type Startup() =
    member _.ConfigureServices (services : IServiceCollection) =
        services.AddGiraffe() |> ignore

    member _.Configure (app : IApplicationBuilder) (_ : IHostEnvironment) (_ : ILoggerFactory) =
        app.UseGiraffe webApp
        
[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(fun whBuilder -> whBuilder.UseStartup<Startup>() |> ignore)
        .Build()
        .Run()
    0