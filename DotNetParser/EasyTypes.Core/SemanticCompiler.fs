namespace DotNetParser

module SemanticCompiler =
    open System
    open System.IO
    open SemanticTypes
    open System.Collections.Generic

    [<Literal>]
    let typesHeader = "types"
    [<Literal>]
    let propsHeader = "primitives"

    let private classifyFiles (folderPath: string): LinkedList<string> * LinkedList<string> =
        let primitiveFiles = LinkedList<string>()
        let typeFiles = LinkedList<string>()
        Directory.EnumerateFiles(folderPath, "*.ty")
        |> Seq.iter (fun f ->
            use st = new StreamReader(f)
            let firstLine = st.ReadLine()
            match firstLine with 
            | "types" ->
                typeFiles.AddLast(f) |> ignore
            | "primitives" ->
                primitiveFiles.AddLast(f) |> ignore
            | _ -> ignore()
             
            )
        primitiveFiles, typeFiles
    
    let primitivesCompile (filePath: string) = 
        File.ReadAllLines(filePath)
        |> Seq.skip 1 //we know the header is primitives
        |> PrimitivesParser.parsePrimitivesLines
        |> ( fun x -> x.customPrimitives |> Seq.map SemanticBuilders.mapPrimitiveToSemantic )

    let typesCompile allPrimitives (filePath: string) = 
        File.ReadAllLines(filePath)
        |> Seq.skip 1 //we know the header is types
        |> CustomTypesParser.parseTypesLines
        |> ( fun x -> x.customTypes |> Seq.map (SemanticBuilders.mapTypeToSemantic allPrimitives) )

    let CompileFolder (folderPath: string) : CustomType seq=
        let primitiveFiles, typeFiles = classifyFiles folderPath
        let allPrimitives = 
            primitiveFiles
            |> Seq.collect primitivesCompile
        let allTypes =
            typeFiles
            |> Seq.collect (typesCompile allPrimitives)
        allTypes
        

