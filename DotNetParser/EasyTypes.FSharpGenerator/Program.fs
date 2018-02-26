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
        (argv.[0], argv.[1], argv.[2], argv.[3])
    else 
        (@"..\..\..\DotNetParser\Samples", @".\GeneratedSamples", "Samples", "Samples")

[<EntryPoint>]
let main argv = 
    let (inputFOlder, outputFolder, coreNamespace, dataNamespace) = parseArgs argv
    let semanticTypes, allPrimitives = SemanticCompiler.CompileFolder inputFOlder 
    semanticTypes 
        |> Seq.iter (fun t ->
            let newFolder = (Path.Combine(outputFolder,"DataProvider"))
            createDir newFolder

            let source = DataProviderGenerator.GenerateAdd dataNamespace t 
            printfn "%s" source
            let formattedSOurce =  Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default
            File.WriteAllText(newFolder + "\\" + t.name + "Data.fs",formattedSOurce)


            ()
        ) 
    semanticTypes 
        |> Seq.iter (fun t ->
            let newFolder = (Path.Combine(outputFolder,"Renditions"))
            createDir newFolder

            let source = RenditionGenerator.Generate coreNamespace t 
            printfn "%s" source
            let formattedSOurce =  Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default
            File.WriteAllText(newFolder + "\\" + t.name + "Rendition.fs",formattedSOurce)


            ()
        ) 
    let writeSource groupName suffix generator (t: 'A when 'A : (member name : string))  = 
    
            let newFolder = (Path.Combine(outputFolder, suffix))
            createDir newFolder

            let source = generator groupName t 
            printfn "%s" source
            let formattedSOurce =  Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default
            File.WriteAllText(newFolder + "\\" + t.name + suffix + ".fs",formattedSOurce)
            ()
    let writePropSource suffix generator (t: CustomPrimitive)  = 
    
            let newFolder = (Path.Combine(outputFolder, suffix))
            createDir newFolder

            let source = generator t 
            printfn "%s" source
            let formattedSOurce =  Fantomas.CodeFormatter.formatSourceString false source FormatConfig.Default
            File.WriteAllText(newFolder + "\\" + t.name + suffix + ".fs",formattedSOurce)
            ()

    semanticTypes 
        |> Seq.iter (writeSource coreNamespace "Domain" DomainGenerator.Generate) 

    allPrimitives 
        |> Seq.iter (writePropSource "Primitive" PrimitiveGeneration.Generate) 
    0 // return an integer exit code
