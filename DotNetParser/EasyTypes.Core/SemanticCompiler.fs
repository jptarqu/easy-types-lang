﻿namespace DotNetParser

module SemanticCompiler =
    open System
    open System.IO
    open SemanticTypes
    open System.Collections.Generic

    [<Literal>]
    let typesHeader = "types"
    [<Literal>]
    let propsHeader = "primitives"

    let private classifyFiles (folderPath: string)  =
        let primitiveFiles = LinkedList<string>()
        let typeFiles = LinkedList<string>()
        let textlookupFiles = LinkedList<string>()
        Directory.EnumerateFiles(folderPath, "*.ty")
        |> Seq.iter (fun f ->
            use st = new StreamReader(f)
            let firstLine = st.ReadLine()
            match firstLine with 
            | "types" ->
                typeFiles.AddLast(f) |> ignore
            | "primitives" ->
                primitiveFiles.AddLast(f) |> ignore
            | "textlookups" ->
                textlookupFiles.AddLast(f) |> ignore
            | _ -> ignore()
             
            )
        textlookupFiles, primitiveFiles, typeFiles 
    
    let lookupsCompile (filePath: string) = 
        File.ReadAllLines(filePath)
        |> Seq.skip 1 //we know the header is primitives
        |> CustomLookupsParser.parseLines
        |> ( fun x -> x.customLookups |> Seq.map (SemanticBuilders.mapLookupsToSemantic ))

    let primitivesCompile allTextLookups (filePath: string) = 
        File.ReadAllLines(filePath)
        |> Seq.skip 1 //we know the header is primitives
        |> PrimitivesParser.parsePrimitivesLines
        |> ( fun x -> x.customPrimitives |> Seq.map (SemanticBuilders.mapPrimitiveToSemantic allTextLookups ))

    let typesCompile allPrimitives (filePath: string) = 
        File.ReadAllLines(filePath)
        |> Seq.skip 1 //we know the header is types
        |> CustomTypesParser.parseTypesLines
        |> ( fun x -> x.customTypes |> Seq.map (SemanticBuilders.mapTypeToSemantic allPrimitives) )

    let CompileFolder (folderPath: string) =
        let textlookupFiles, primitiveFiles, typeFiles = classifyFiles folderPath
        let allTextLookups =
            textlookupFiles
            |> Seq.collect lookupsCompile
            |> Seq.toArray
        let allPrimitives = 
            primitiveFiles
            |> Seq.collect (primitivesCompile allTextLookups)
            |> Seq.toList
        let allTypes =
            typeFiles
            |> Seq.collect (typesCompile allPrimitives)
        allTypes |> Seq.toList, allPrimitives 
        

