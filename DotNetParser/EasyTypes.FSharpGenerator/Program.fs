open DotNetParser
open System.IO
open EasyTypes.FSharpGenerator
open Fantomas.FormatConfig
open Fantomas
open DotNetParser.SemanticTypes

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

let createDir (folder:string) =
    if (not (Directory.Exists folder)) then
        Directory.CreateDirectory folder |> ignore
        
let parseArgs (argv:string array) =
    if (argv.Length > 0) then
        (argv.[0], argv.[1], argv.[2])
    else 
        (@"..\..\..\DotNetParser\Samples", @".\GeneratedSamples", "Samples")

[<EntryPoint>]
let main argv = 
    let (inputFOlder, outputFolder, groupName) = parseArgs argv
    let semanticTypes = SemanticCompiler.CompileFolder inputFOlder |> Seq.toList
    semanticTypes 
        |> Seq.iter (fun t ->
            let newFolder = (Path.Combine(outputFolder,"DataProvider"))
            createDir newFolder

            let source = DataProviderGenerator.GenerateAdd groupName t 
            printfn "%s" source
            let formattedSOurce =  Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default
            File.WriteAllText(newFolder + "\\" + t.name + "Data.fs",formattedSOurce)


            ()
        ) 
    semanticTypes 
        |> Seq.iter (fun t ->
            let newFolder = (Path.Combine(outputFolder,"Renditions"))
            createDir newFolder

            let source = RenditionGenerator.Generate groupName t 
            printfn "%s" source
            let formattedSOurce =  Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default
            File.WriteAllText(newFolder + "\\" + t.name + "Rendition.fs",formattedSOurce)


            ()
        ) 
    let writeSource groupName suffix generator (t: CustomType)  = 
    
            let newFolder = (Path.Combine(outputFolder, suffix))
            createDir newFolder

            let source = generator groupName t 
            printfn "%s" source
            let formattedSOurce =  Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default
            File.WriteAllText(newFolder + "\\" + t.name + suffix + ".fs",formattedSOurce)
            ()

    semanticTypes 
        |> Seq.iter (writeSource groupName "Domain" DomainGenerator.Generate) 
    0 // return an integer exit code
