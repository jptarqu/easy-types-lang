// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open EasyTypes.SqlGenerator
open DotNetParser
open System.IO


let createDir (folder:string) =
    if (not (Directory.Exists folder)) then
        Directory.CreateDirectory folder |> ignore

[<EntryPoint>]
let main argv = 

    let semanticTypes = SemanticCompiler.CompileFolder @".\Samples" |> Seq.toList
    semanticTypes 
        |> Seq.iter (fun t ->
            createDir "Tables"
            
            File.WriteAllText("Tables\\" + t.name + ".sql", SqlTableGenerator.buildTable t |> SqlCommon.FormatSql)
        ) 
    semanticTypes 
        |> Seq.iter (fun t ->
            createDir "StoredProcedures"
            File.WriteAllText("StoredProcedures\\spInsert" + t.name + ".sql", SqlStoredProcGenerator.buildInsertSp t |> SqlCommon.FormatSql)
        ) 

    0 // return an integer exit code
