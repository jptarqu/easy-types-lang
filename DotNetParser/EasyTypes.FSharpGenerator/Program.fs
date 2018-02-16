open DotNetParser
open System.IO
open EasyTypes.FSharpGenerator
open Fantomas.FormatConfig
open Fantomas

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

let createDir (folder:string) =
    if (not (Directory.Exists folder)) then
        Directory.CreateDirectory folder |> ignore

[<EntryPoint>]
let main argv = 
    let groupName = if argv.Length > 0 then argv.[0] else @"Samples"
    let semanticTypes = SemanticCompiler.CompileFolder (@".\" + groupName) |> Seq.toList
    semanticTypes 
        |> Seq.iter (fun t ->
            createDir "DataProvider"
            let source = DataProviderGenerator.GenerateAdd groupName t 
                            |> (fun source -> Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default)
            File.WriteAllText("DataProvider\\" + t.name + "Data.fs",source)
            ()
        ) 
    0 // return an integer exit code
