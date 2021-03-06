﻿// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System.IO
open DotNetParser
open System
open DotNetParser.SemanticBuilders

[<EntryPoint>]
let main argv = 

    let semanticTypes = SemanticCompiler.CompileFolder @".\Samples" |> Seq.toList

    for t in semanticTypes do
        printfn "%A" t

    Console.ReadLine() |> ignore
    0 // return an integer exit code
