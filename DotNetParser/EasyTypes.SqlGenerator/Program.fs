// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open EasyTypes.SqlGenerator
open DotNetParser
open System.IO
[<EntryPoint>]
let main argv = 
    let semanticTypes = SemanticCompiler.CompileFolder @".\Samples" |> Seq.toList
    let tablesFileContents = semanticTypes |> Seq.map SqlTableGenerator.buildTable |> String.concat "\n"
    File.WriteAllText("sqlTables.sql",tablesFileContents)
    printfn "%A" tablesFileContents
    0 // return an integer exit code
