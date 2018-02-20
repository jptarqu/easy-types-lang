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
        
let parseArgs (argv:string array) =
    if (argv.Length > 0) then
        (argv.[0], argv.[1], argv.[2])
    else 
        (@".\Samples", @".\GeneratedSamples", "Samples")

[<EntryPoint>]
let main argv = 
    let (inputFOlder, outputFolder, groupName) = parseArgs argv
    let groupName = if argv.Length > 0 then argv.[0] else @"Samples"
    let semanticTypes = SemanticCompiler.CompileFolder inputFOlder |> Seq.toList
    semanticTypes 
        |> Seq.iter (fun t ->
            let newFolder = (Path.Combine(outputFolder,"DataProvider"))
            createDir newFolder

            let source = DataProviderGenerator.GenerateAdd groupName t 
                            |> (fun source -> Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default)
            File.WriteAllText(newFolder + "\\" + t.name + "Data.fs",source)
            ()
        ) 
    0 // return an integer exit code
