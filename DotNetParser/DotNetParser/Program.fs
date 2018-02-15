// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System.IO
open DotNetParser
open System
open DotNetParser.SemanticBuilders

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let primitivesParseInfo = 
        File.ReadAllLines(@"Samples\samplePrimitives.ty") 
        |> Seq.skip 1 //we know the header is primitives
        |> PrimitivesParser.parsePrimitivesLines
    
    let customTypesParseInfo = 
        File.ReadAllLines(@"Samples\sampleTypes.ty")
        |> Seq.skip 1 //we know the header is types
        |> CustomTypesParser.parseTypesLines

        
    let basicPrimitives = 
        File.ReadAllLines(@"BuiltInPrimitives.ty")
        |> Seq.skip 1 //we know the header is primitives
        |> PrimitivesParser.parsePrimitivesLines
        |> ( fun x -> x.customPrimitives |> Seq.map SemanticBuilders.mapPrimitiveToSemantic )
    let semanticProps = primitivesParseInfo.customPrimitives |> Seq.map SemanticBuilders.mapPrimitiveToSemantic 
    let allPrimitves = [ semanticProps ; basicPrimitives ] |> Seq.concat

    let semanticTypes = customTypesParseInfo.customTypes |> Seq.map (SemanticBuilders.mapTypeToSemantic allPrimitves)

    for t in semanticTypes do
        printfn "%A" t

    Console.ReadLine() |> ignore
    0 // return an integer exit code
