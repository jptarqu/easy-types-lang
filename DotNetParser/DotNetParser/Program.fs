// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System.IO
open DotNetParser
open System

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let primitivesParseInfo = 
        File.ReadAllLines(@"Samples\samplePrimitives.ty") 
        |> PrimitivesParser.parsePrimitivesLines
    
    let customTypesParseInfo = 
        File.ReadAllLines(@"Samples\sampleTypes.ty") 
        |> CustomTypesParser.parseTypesLines
    let semanticProps = primitivesParseInfo.customPrimitives |> Seq.map SemanticTypes.mapToSemantic 
    let semanticTypes = customTypesParseInfo.customTypes |> Seq.map (SemanticTypes.mapTypeToSemantic semanticProps)

    for t in semanticTypes do
        printfn "%A" t

    Console.ReadLine() |> ignore
    0 // return an integer exit code
